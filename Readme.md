# Çiceksepeti Aday Test Uygulaması


## Uygulamanın Amacı


> Uygulama Çiçeksepeti için bir Sepete Ekle servisi oluşturmayı amaçlar.

## Uygulamanın İçeriği

- ### Kütüphaneler

  - #### Basket.Caching
    Uygulamada kullanılan Caching işlemleri için kullanılacak tipleri barındırır. Uygulamada bir çeşit tanımlanmıştır.

    - ##### InMemoryCache
        Uygulama boyunca memory üzerinden çalışan Cache mekanizmasıdır.
  
   - #### Basket.Data
        Uygulamada kullanılan sınıfları ve arayüzleri barındırır.

        - ##### Cart

            Sepet sınıfıdır.

        - ##### Product

            Ürün sınıfıdır.

        - ##### IEntity

            Sınıfların türediği arayüzdür. İçerisindeki Id değeri generic olarak tanımlanır. Ve bu değer ilgili sınıfın tanımlayıcı özelliğidir.

    - #### Basket.IoC
    
        Uygulama boyunca kullanılan IoC Container tanımını barındırır.[^1]
    
    - #### Basket.Library
    
        Uygulamada boyunca ortak kullanılacak sınıfların tanımını barınıdırır.

    - #### Basket.Logging

        Uygulamanın loglama mekanizmasını barındırır.[^2]
    
    - #### Basket.Logging
    
        Uygulamanın loglama mekanizmasını barındırır.

    - #### Basket.Repository
    
        Uygulamanın veri erişimi için gerekli sınıflarını barındırır. Uygulamada iki çeşit tanımlanmıştır yapılmıştır.
        
        - ##### MongoContext
        
            MongoDb veri tabanına veri erişimi sağlar.[^3]
        
        - ##### InMemoryContext
            Memory üzerinde çalışan veri tabanına veri erişimi sağlar.
        
    - #### Basket.Service

        - Uygulamada kullanılacak iş servislerini ve bunların fonksiyonlarını belirleyen arayüzleri barındırır. 
        - İş servislerinin uygulama boyunca kullanılacak kesişen ilgilerin yapılabilmesi için imkan sağlayan proxy mekanizmasını barındırır. 

- ### Testler   
    - #### Basket.Test

        - Uygulamada koşacak servis testlerini barındırır.
        - Test koşturulduğu anda kayıtlar oluşturulur ve servis çağrımları yapılır.
        - Kayıtlar için InMemoryDbContext oluşturulur.
        
    - #### Basket.Http

        - Uygulamada koşacak api testlerini barındırır.
        - Test koşmadan önce;
            - Test sunucusu ayağa kaldırılır.
            - Ürün ve sepet kayıtları oluşturulur.
        
- ### Basket.API

    Uygulamanın erişim sağlanan, sepete ekleme işlemini barındıran API'sini barındırır. 
    
- ### Docker

    - Uygulamanın derlemesi, testlerinin yapılması ve dağıtımı Docker yardımıyla yapılmaktadır.
    - Tanımlar, Dockerfile ve docker-compose.yml içerisinde yapılmıştır.

    - #### docker-compose.yml
        
        Container üzerinde ayağa kaldırılacak uygulamalar tanımını barındırır. 

        - Ayağa kaldırılacak uygulamalar şunlardır:

            - mongo 

                MongoDb resmi uygulaması
            - mongoexpress
            
                MongoDb yönetim paneli uygulaması
            - app-server
            
                Basket Aday Test uygulaması 

    - #### Dockerfile
        
        Basket Aday Test uygulamasının derleme, test ve dağıtma tanımlarını barındıran dosyadır. 

        - Uygulama klasörleri, çalışma klasörüne kopyalanır.
        - Uygulamada kullanılacak nuget paketleri yüklenir.
        - Uygulamadaki projeler derlenir.
        - Uygulamadaki testler koşturulur.[^4]
        - Test sonucu başarılı olduğu durumda uygulamanın dağıtımı yapılır
        - Uygulama çalıştırılır.[^5]

        

[^1]: IoC Container olarak SimpleInjector kullanılmaktadır.
[^2]: Log işlemleri için NLog kütüphanesi kullanılmaktadır. **NLog.config** dosyasıyla çalışmaktadır. İlgili dosya testler ve uygulamada ekli durumdadır. Varsayılan olarak, uygulama klasörünün altındaki **Log.txt** adında bir dosyaya yazma işlemi yapmaktadır. 
[^3]: Bağlantı bilgileri uygulama çalışma klasöründeki **appsettings.json** içerisinde **ConnectionStrings** altında bulunmaktadır. Varsayılan olarak **DefaultConnection** değeri kullanılmaktadır. İlgili dosya testler ve uygulamada ekli durumdadır.
[^4]: Basket.Test projesi altındaki testler koşturulmaktadır. 
[^5]: Uygulama 80 ve 443 portu üzerinden çalışır.