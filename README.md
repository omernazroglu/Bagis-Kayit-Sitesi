<div align="center">

# 🤝 UFUKDER Bağış Kayıt Sistemi

### Dernek Bağışlarını Yönetmek için Geliştirilmiş Web Platformu

<br/>

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity_Framework_Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-Destekli-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Unit of Work](https://img.shields.io/badge/Unit_of_Work-Pattern-7C3AED?style=for-the-badge&logoColor=white)

<br/>

<p align="center">
  <b>UFUKDER derneğinin bağış süreçlerini dijitalleştiren; bağışçı kaydı, bağış takibi, gruplama ve log yönetimi sunan kapsamlı bir yönetim platformu.</b>
</p>

</div>

---

## 🎯 Proje Hakkında

Bu sistem, UFUKDER derneğine yapılan bağışları kayıt altına almak, bağışçıları gruplamak ve tüm değişiklik geçmişini log dosyaları üzerinden izlemek amacıyla geliştirilmiştir.

**Temel ihtiyaçlar:**
- Bağışçı bilgilerini (ad-soyad, telefon, referans vb.) esnek bir yapıyla kaydetme
- Bağışları gruplara ayırarak organize etme
- Her bağış üzerinde yapılan değişiklikleri log dosyasından okuma
- Referans kişi ile bağışı ilişkilendirme

---

## ✨ Özellikler

<table>
<tr>
<td width="50%">

### 📋 Bağış Yönetimi
- Yeni bağış kaydı oluşturma
- Bağışçı bilgilerini esnek sütun yapısıyla kaydetme
- Bağışı aktif/pasif duruma alma
- Referans kişi ile bağış ilişkilendirme
- Bağış detaylarını güncelleme (JSON AJAX)

</td>
<td width="50%">

### 👥 Gruplama Sistemi
- Birden fazla bağışı aynı grup numarasıyla gruplama
- Grup görüntüleme sayfası
- Bağışçı adı ve telefon bilgisini grup bazında listeleme
- Çoklu seçim ile toplu gruplama

</td>
</tr>
<tr>
<td width="50%">

### 📜 Log Takibi
- Günlük log dosyalarından otomatik okuma (`Logs/*.txt`)
- Belirli bir bağışa ait değişiklik geçmişini görüntüleme
- Tarih/saat, sütun ve açıklama bazlı log analizi
- Partial View ile dinamik log paneli

</td>
<td width="50%">

### 🏗️ Yazılım Mimarisi
- **Repository Pattern** ile veritabanı soyutlama
- **Unit of Work** ile transaction yönetimi
- **Generic Service** katmanı
- Interface tabanlı bağımlılık enjeksiyonu
- Async/Await ile asenkron işlemler

</td>
</tr>
</table>

---

## 🏛️ Mimari Yapı

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                     │
│              HomeController  →  Views                    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                   Service Layer                          │
│         IService<T>  →  Service<T> (Generic)            │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              Unit of Work + Repository Layer             │
│   IUnitOfWork  →  UnitOfWork                           │
│   IRepository<T>  →  Repository<T> (Generic)           │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│               Data Layer (EF Core)                       │
│                   AppDbContext                           │
│              SQL Server Veritabanı                      │
└─────────────────────────────────────────────────────────┘
```

---

## 🗃️ Veritabanı Modeli

```
KULLANICI
├── Id (PK)
└── ...
    │
    └──▶ BAGISLAR
         ├── id (PK)
         ├── KULLANICI_ID (FK)
         ├── ReferansId (FK → REFERANSLAR)
         ├── OLUSTURMA_TARIHI
         ├── DEGISTIRME_TARIHI
         └── AKTIF
              │
              ├──▶ BAGIS_BILGILERI
              │    ├── id (PK)
              │    ├── BAGISLAR_ID (FK)
              │    ├── SUTUNLAR_ID (FK → REF_SUTUNLAR)
              │    └── ACIKLAMA
              │
              └──▶ BAGIS_GRUP
                   ├── id (PK)
                   ├── BAGISLAR_ID (FK)
                   └── GRUP_NO

REF_SUTUNLAR          REFERANSLAR
├── id (PK)           ├── Id (PK)
└── ACIKLAMA          └── Ad_Soyad
```

> **Esnek Sütun Yapısı:** Bağış detayları sabit kolonlar yerine `REF_SUTUNLAR` referans tablosuyla dinamik olarak tanımlanır. Örnek: `SUTUNLAR_ID=1` → Ad Soyad, `SUTUNLAR_ID=2` → Telefon, `SUTUNLAR_ID=7` → Referans kişi

---

## 🛠️ Teknoloji Yığını

| Katman | Teknoloji | Açıklama |
|--------|-----------|----------|
| **Backend** | ASP.NET Core 8 MVC | Web framework |
| **ORM** | Entity Framework Core 8 | Code-First / DB-First |
| **Veritabanı** | Microsoft SQL Server | İlişkisel veritabanı |
| **Mimari** | Repository + Unit of Work | Tasarım desenleri |
| **Async** | async/await | Asenkron veritabanı işlemleri |
| **API** | JSON (AJAX) | AJAX ile bağış güncelleme |
| **Log** | Dosya tabanlı (`.txt`) | Günlük log takibi |
| **Frontend** | Bootstrap 5, jQuery | Responsive arayüz |

---

## 📁 Proje Yapısı

```
UFUKDER_BAGIS/
├── Controllers/
│   └── HomeController.cs         # Ana controller — tüm işlemler
│
├── Models/                        # EF Core Entity modelleri
│   ├── AppDbContext.cs            # Veritabanı bağlamı
│   ├── Bagislar.cs                # Ana bağış tablosu
│   ├── BagisBilgileri.cs         # Bağış detay bilgileri
│   ├── BagisGrup.cs              # Bağış gruplama
│   ├── Referanslar.cs            # Referans kişiler
│   ├── RefSutunlar.cs            # Dinamik sütun tanımları
│   ├── Kullanici.cs              # Kullanıcı modeli
│   ├── Bagisci.cs                # Bağışçı bilgi modeli
│   ├── BagislarViewModel.cs      # Listeleme için ViewModel
│   ├── LoglarViewModel.cs        # Log görüntüleme ViewModel
│   └── Result.cs                 # Genel işlem sonucu sarmalayıcı
│
├── Services/
│   ├── Interfaces/
│   │   ├── IRepository.cs        # Generic repository arayüzü
│   │   ├── IService.cs           # Generic service arayüzü
│   │   └── IUnitOfWork.cs        # Unit of Work arayüzü
│   └── Concrete/
│       ├── Repository.cs         # Generic repository implementasyonu
│       ├── Service.cs            # Generic service implementasyonu
│       └── UnitOfWork.cs         # Transaction yönetimi
│
├── Views/
│   └── Home/
│       ├── Index.cshtml          # Ana sayfa — bağış listesi
│       ├── GrupGoruntule.cshtml  # Grup bazlı bağışçı listesi
│       └── Deneme.cshtml         # Log görüntüleme (Partial View)
│
├── Logs/                         # Otomatik oluşan log dosyaları
│   └── log-YYYYMMDD.txt
│
├── wwwroot/
│   ├── css/bagislar.css          # Bağış sayfası stilleri
│   └── js/bagislar.js            # AJAX ve interaktif işlemler
│
├── appsettings.json              # Veritabanı bağlantı ayarları
└── Program.cs                    # Uygulama başlangıç noktası
```

---

## 🚀 Kurulum & Çalıştırma

### Ön Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/tr-tr/sql-server/sql-server-downloads)
- [Git](https://git-scm.com/)

### Adım Adım

**1. Repoyu klonlayın:**
```bash
git clone https://github.com/omernazroglu/Bagis-Kayit-Sitesi.git
cd Bagis-Kayit-Sitemi
```

**2. Bağlantı dizesini güncelleyin** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=UFUKDER_BAGIS;Trusted_Connection=True;"
  }
}
```

**3. Veritabanını oluşturun:**
```bash
dotnet ef database update
```

**4. Uygulamayı başlatın:**
```bash
dotnet run
```

Tarayıcınızda açın → **http://localhost:5000**

---

## 🔌 API Endpoint'leri

| Method | URL | Açıklama |
|--------|-----|----------|
| `GET` | `/` | Bağış listesi ana sayfası |
| `GET` | `/Home/GrupGoruntule` | Grup bazlı bağışçı listesi |
| `GET` | `/Home/GetLogs?id={bagisId}` | Bağışa ait log geçmişi |
| `POST` | `/Home/GeriDon` | Bağış bilgilerini güncelle (JSON) |
| `POST` | `/Home/Grupla` | Seçili bağışları grupla (JSON) |
| `POST` | `/Home/Pasif?id={bagisId}` | Bağışı pasif yap |

---

## 📜 Log Dosyası Formatı

Log dosyaları `Logs/log-YYYYMMDD.txt` şeklinde tutulur. Her satır:

```
2026-02-26 14:35:22 INFO  ID: 42 1 Ahmet Yilmaz
                                  ↑ ↑ ↑___________
                                  │ │ Açıklama
                                  │ SutunlarId (1=AdSoyad, 2=Telefon...)
                                  BagislarId
```

---

## 🔮 Geliştirme Fikirleri

- [ ] Kullanıcı girişi ve yetkilendirme sistemi
- [ ] Bağış raporu PDF/Excel çıktısı
- [ ] Dashboard — toplam bağış, aktif/pasif istatistikleri
- [ ] Bağışçı arama ve filtreleme
- [ ] E-posta ile bağış bildirimi
- [ ] Mobil uyumlu arayüz iyileştirmeleri

---

## 📄 Lisans

Bu proje [MIT](LICENSE) lisansı altında dağıtılmaktadır.

---

<div align="center">

**⭐ Projeyi beğendiysen yıldız vermeyi unutma!**

*UFUKDER için geliştirildi* 🤝

</div>
