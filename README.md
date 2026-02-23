# BarnManagementWinFormApi

.NET 9.0 kullanılarak geliştirilmiş, çiftlik yönetim uygulaması.

## Özellikler:
- **Rol Yönetimi:**
	- Admin, User adında iki adet kullanıcı bulunur.
	- Kullanıcı giriş sistemi ile kullanıcı giriş yapar
	- Admin, yeni kullanıcı oluşturma ve listeleme yetkisine sahiptir.
- **Çiftlik Yönetimi:**
	- Her kullanıcının bir adet çiftliği bulunur
	- Kullanıcıya tanımlı çiftliğe hayvan alımı yapılabilir.
- **Hayvan Yönetimi:**
	- Çiftliğe alınan her hayvan 3 ay yaşında alınır.
	- Çiftlikte bulunan hayvanların yaşı 6 ayı geçtikten sonra üretime başlarlar.
	- Her hayvan, türüne bağlı olarak yaşam süresini doldurduktan sonra ölür.
- **Üretim Yönetimi:**
	- Her hayvan, türüne bağlı olarak bir adet ürün üretir.
	- Çiftlikte bulunan hayvan sayısına göre üretim miktarı artar.
	- Üretime uygun olan hayvanlar üretim listesine alınır.
	- Üretime uygun olmayan hayvanlar üretim listesinden çıkarılır.
- **Envanter Yönetimi:**
	- Hayvanlar üretim yaptıktan sonra cart'ta biriken ürünler envantere alınır.
	- Envanterde bulunan her ürün istenilen adette satılabilir.
- **Çalışan Yönetimi:**
	- Üretilen ürünleri envantere taşımak için çalışanlar kullanılır.
	- Her çalışanın belirli bir kapasitesi bulunur, bu kapasite ücret karşılığı arttırılabilir
	- Her çalışanın belirli bir çalışma süresi bulunur, bu süre ücret karşılığı hızlandırılabilir.
	- Çiftliğe alınan çalışan, daha düşük bir ücretle satılabilir.
- **Hata Yönetimi ve Loglama:**
	- Alınan hatalar Logs dosyasına kaydedilir.

## Mimari:

- **Business:** İş mantığının, kuralların ve veri işleme süreçlerinin yönetildiği ana katman.
    - **Abstract:** Servis arayüzlerinin tanımlandığı alan.
    - **Constants:** Proje genelinde kullanılan sabit mesaj ve değerlerin bulunduğu klasör.
    - **DTOs:** Katmanlar arası veri transferinde kullanılan nesnelerin bulunduğu alan.
    - **Profiles:** Automapper eklentisinin kontrol edildiği profil.
    - **Services:** İş mantığının kodlandığı ve hesaplamaların yapıldığı sınıflar.
-  **Client:** Kullanıcının etkileşime girdiği arayüz katmanı.
    - **Pages:** Uygulama içerisindeki panellerin ve kullanıcı kontrollerinin bulunduğu alan.
    - **Forms:** Login ve MainForm gibi ana pencerelerin yönetildiği sınıflar.
- **Core:** Projenin genel altyapı ve yardımcı bileşenlerinin bulunduğu katman.
    - **Logging:** Uygulama içi hataların ve olayların kayıt altına alındığı loglama mekanizması.
- **DataAcces:** Veritabanı bağlantısı ve veri erişim işlemlerinin yapılandırıldığı katman.
- **Entities:** Veritabanı tablolarını temsil eden ana varlıkların tanımlandığı katman.
- **WebAPI:** Sunucu tarafındaki işlemlerin client ile bağlandığı katmandır.
	- **Controllers:** Gelen HTTP isteklerini alan ve business katmanına yönlendiren sınıf.
	- **BackgroundJobs:** Arka planda çalışan işleri yöneten sınıf.

        

## Kullanılan Teknolojiler:
- .NET 9.0
- C# Windows Form
- .NET Core Web Api
- **NuGet paketleri:**
	- AutoMapper
	- AspNetCore.Identity
	- EntityFrameworkCore.SqlServer
	- EntityFrameworkCore.Tools
	- EntityFrameworkCore.Design
	- AspNetCore.Authentication.JwtBearer
	- IdentityModel.Tokens
	- Swashbuckle.AspNetCore
	- IdentityModel.Tokens.Jwt

## Kurulum:
- Repoyu klonlayın
	 `git clone https://github.com/Omercanonen/BarnManagement`
- Projeyi Visual Studio 2022 ile açın
- Gerekli NuGet paketlerini indirin
- Projeyi başlatın

## Klasör Yapısı:
```
BarnManagementWin
├── Business
│   ├── Abstract
│   │   ├── ApiServices
│   ├── Constants
│   ├── DTOs
│   │   ├── Auth
│   │   ├── Barn
│   ├── Profiles
│   ├── Security
│   └── Services
│       ├── ApiServices
├── Client
│   ├── Pages
│   ├── Login.cs
│   ├── MainForm.cs
│
├── Core
│   └── Logging
├── DataAccess
│   ├── Context
│   └── Migrations
├── Entities
│   └── Concrete
├── WebAPI
│   ├── BackgroundJobs
│   ├── Controllers
│   ├── Program.cs
│   └── WebAPI.http        
```
