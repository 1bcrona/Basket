using Castle.DynamicProxy;
using Basket.Caching.Interface;
using Basket.IoC;
using Basket.Logging;
using Basket.Logging.Interface;
using Basket.Service.Attribute;
using Basket.Service.Exception.Base;
using Basket.Service.Exception.Impl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Service.Interception
{
    public class ServiceInterceptor : IAsyncInterceptor
    {
        #region Private Fields

        private static ICache _Cache = ContainerHandler.Instance.Container.GetInstance<ICache>();

        #endregion Private Fields

        #region Private Properties

        private static ILogger _Logger => ContainerHandler.Instance.Container.GetInstance<ILogger>();

        #endregion Private Properties

        #region Public Methods

        public void InterceptAsynchronous(IInvocation invocation)
        {
            System.Exception exception = null;
            var parameters = GetParameters(invocation.Method.GetParameters(), invocation.Arguments);
            object ret = null;
            try
            {
                invocation.ReturnValue = InternalInterceptAsynchronous(invocation);

                if (!(invocation.ReturnValue is Task { IsFaulted: true } res)) return;
                if (res.Exception != null)
                    throw res.Exception;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                ret = ex;
            }
            finally
            {
                parameters["___Return___"] = ret;
                _Logger.Log(new LogEntry(LogType.Info, JsonConvert.SerializeObject(parameters)));
                if (exception != null)
                    throw exception;
            }
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            System.Exception exception = null;
            var parameters = GetParameters(invocation.Method.GetParameters(), invocation.Arguments);
            object ret = null;

            var invocationKey = string.Empty;

            try
            {
                var sb = new StringBuilder();

                sb.Append(invocation.TargetType.FullName);
                sb.Append(":");
                sb.Append(invocation.Method.Name);
                sb.Append(":");
                sb.Append("Args");
                sb.Append("=");

                var arguments = parameters.Select(keyValuePair => string.Join(":", keyValuePair.Key, keyValuePair.Value)).ToList();

                var argumentKey = arguments.Count > 0 ? string.Join("|", arguments) : "null";
                sb.Append(argumentKey);

                invocationKey = sb.ToString();

                if (invocationKey.EndsWith("|")) invocationKey = invocationKey.Substring(0, invocationKey.Length - 1);

                var cacheAttrOfMethod = invocation.Method.GetType().GetCustomAttribute<CacheAttribute>();

                var invokeMethod = true;

                if (cacheAttrOfMethod != null)
                {
                    var cachedObject = GetResultFromCache<TResult>(invocationKey);
                    if (cachedObject != null)
                    {
                        ret = cachedObject;
                        invocation.ReturnValue = Task.FromResult(cachedObject);
                        invokeMethod = false;
                    }
                }

                if (invokeMethod)
                {
                    invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
                    var res = invocation.ReturnValue as Task<TResult>;
                    if (res == null) return;

                    if (res.IsFaulted)
                    {
                        if (res.Exception?.InnerException != null)
                            throw res.Exception.InnerException;
                    }

                    ret = res.Result;
                    parameters["___Return___"] = ret;
                }

                if (cacheAttrOfMethod != null)
                {
                    SetResultToCache(invocationKey, ret, cacheAttrOfMethod.TimeSpan);
                }
            }
            catch (System.Exception ex)
            {
                exception = (ex as ServiceException ?? ex.InnerException as ServiceException) ?? new UnknownErrorException(ex);
                parameters["___Return___"] = exception;
            }
            finally
            {
                parameters["__Invocation_Key__"] = invocationKey;
                _Logger.Log(new LogEntry(LogType.Info, JsonConvert.SerializeObject(parameters)));
                if (exception != null)
                    throw exception;
            }
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            System.Exception exception = null;
            var parameters = GetParameters(invocation.Method.GetParameters(), invocation.Arguments);
            object ret = null;
            try
            {
                invocation.Proceed();
                ret = invocation.ReturnValue;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                ret = ex;
            }
            finally
            {
                parameters["___Return___"] = ret;
                _Logger.Log(new LogEntry(LogType.Info, JsonConvert.SerializeObject(parameters)));
                if (exception != null)
                    throw exception;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetHash(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                var md5 = MD5.Create();
                str = Encoding.UTF8.GetString(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
            }
            return str ?? string.Empty;
        }

        private string GetArgumentKey(object arg)
        {
            if (arg == null) return "null";
            var type = arg.GetType();
            var tc = Type.GetTypeCode(type);
            string argString;

            switch (tc)
            {
                case TypeCode.String:
                    {
                        argString = (string)arg;
                        if (argString.Length > 24)
                        {
                            argString = GetHash(argString);
                        }

                        break;
                    }
                case TypeCode.DateTime:
                    argString = ((DateTime)arg).Ticks.ToString();
                    break;

                default:
                    {
                        if (type == typeof(DateTime?))
                        {
                            argString = ((DateTime?)arg).Value.Ticks.ToString();
                        }
                        else if (tc != TypeCode.Object)
                        {
                            argString = arg.ToString();
                        }
                        else
                        {
                            argString = JsonConvert.SerializeObject(arg);
                            if (!String.IsNullOrEmpty(argString))
                            {
                                argString = GetHash(argString);
                            }
                        }

                        break;
                    }
            }

            return argString;
        }

        private Dictionary<string, object> GetParameters(ParameterInfo[] parameterInfos, object[] args)
        {
            var parameters = new Dictionary<string, object>();
            for (var i = 0; i < args.Length; i++)
            {
                parameters[parameterInfos[i].Name ?? string.Empty] = GetArgumentKey(args[i]);
            }

            return parameters;
        }

        private TResult GetResultFromCache<TResult>(string invocationKey)
        {
            var cachedObject = _Cache.Get(invocationKey, default(TResult)).Result;
            return cachedObject;
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            var result = await task;
            return result;
        }

        private void SetResultToCache(string invocationKey, object data, TimeSpan timeSpan)
        {
            _Cache.Set(invocationKey, data, timeSpan);
        }

        #endregion Private Methods
    }
}