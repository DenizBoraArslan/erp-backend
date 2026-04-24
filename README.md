# erp-backend

Bu repository’de her servis kendi veritabanı ve migration yapısına sahiptir.

## Kimlik Doğrulama ve Yetkilendirme

- JWT tabanlı authentication tüm API servislerinde etkinleştirildi.
- `User.API` içinde aşağıdaki endpointler eklendi:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `POST /api/auth/refresh-token`
- Role-based authorization aktif:
  - Yazma işlemleri için `Admin` / `Manager`
  - `Stock` ürün okuma için `Admin` / `Manager` / `User` / `Viewer`

## Servis İletişimi Temeli

- Senkron iletişim altyapısı için `HttpClient` merkezi olarak kayıt edilir (`AddOperationalFoundation`).
- Asenkron iletişim temeli için in-memory integration event publisher/processor eklendi.
- User kayıt akışında `UserRegisteredIntegrationEvent` yayınlanır.

## Operasyonel Temel

- Her API’de `/health` endpointi tanımlandı.
- `X-Correlation-Id` destekli request correlation middleware eklendi.
- Integration event processing merkezi logger ile loglanır.

## Gereksinimler

- .NET SDK 10.0+
- SQL Server / LocalDB
- `dotnet-ef` aracı

## dotnet-ef kurulumu

```bash
dotnet tool install --global dotnet-ef
```

## Migration oluşturma

Örnek (Account servisi):

```bash
dotnet ef migrations add <MigrationAdi> \
  --project Services/Account/Account.Infrastructure/Account.Infrastructure.csproj \
  --startup-project Services/Account/Account.API/Account.API.csproj \
  --context Account.Infrastructure.Persistence.AccountDbContext \
  --output-dir Persistence/Migrations
```

Aynı pattern, diğer servisler için ilgili Infrastructure/API projesi ve DbContext ile uygulanır.

## Veritabanı oluşturma / güncelleme

Migration’lar startup sırasında otomatik uygulanır (`Database.Migrate()`).
Bu nedenle ilgili API ayağa kaldırıldığında veritabanı otomatik oluşturulur/güncellenir.

Alternatif olarak manuel komut:

```bash
dotnet ef database update \
  --project Services/Account/Account.Infrastructure/Account.Infrastructure.csproj \
  --startup-project Services/Account/Account.API/Account.API.csproj \
  --context Account.Infrastructure.Persistence.AccountDbContext
```

## Şimdilik Test Ederken İzlenecek Adımlar

> Not: Çözüm içinde şu an ayrı bir otomatik test projesi bulunmadığından aşağıdaki adımlar smoke test / manuel doğrulama içindir.

1. **Projeyi derle**
   ```bash
   dotnet build ErpBackend.sln
   ```

2. **Mevcut test komutunu çalıştır**
   ```bash
   dotnet test ErpBackend.sln
   ```

3. **Test edilecek servisi ayağa kaldır**
   ```bash
   dotnet run --project Services/Account/Account.API/Account.API.csproj
   ```
   Aynı komut, ilgili servis yoluna göre diğer API’ler için de uygulanır (`Sales.API`, `Stock.API`, `HR.API`, `Purchase.API`, `User.API`).

4. **Servis açıldıktan sonra temel kontrol yap**
   - API açılış loglarında servis dinlemeye başladığını gösteren mesajı (örn. `Now listening on ...`) ve hata olmamasını doğrula.
   - Swagger/OpenAPI ekranının açıldığını doğrula.
   - En az bir endpoint’e istek atıp `2xx` yanıt alındığını kontrol et.

5. **Veritabanı kontrolü**
   - İlk açılışta migration’ların otomatik uygulandığını loglardan doğrula.
   - Gerekirse `dotnet ef database update` ile manuel güncelleme yap ve servisi tekrar başlatıp endpoint’leri tekrar test et.

## Önerilen Task Listesi (Backlog)

### 1) Mimari ve Ortak Altyapı
- [ ] API versioning standardını tüm servislerde tanımla.
- [ ] Merkezi logging (Serilog + structured log) ve log correlation standartlarını genişlet.
- [ ] Servisler arası iletişim için retry/circuit-breaker politikalarını (`Polly`) ortaklaştır.
- [ ] Distributed tracing (OpenTelemetry) ekleyip temel dashboard görünürlüğü sağla.
- [ ] Config yönetimi için environment bazlı doğrulama ve gizli bilgi (secret) stratejisini netleştir.

### 2) Güvenlik ve Kimlik Yönetimi
- [ ] JWT key/issuer/audience yapılandırmaları için zorunlu doğrulamalar ve startup fail-fast kontrolleri ekle.
- [ ] Refresh token ömrü, rotasyonu ve revoke senaryolarını netleştir; blacklisting desteği ekle.
- [ ] Role/permission modelini claim bazlı yetkilendirme politikalarına dönüştür.
- [ ] API rate limiting ve brute-force koruması (özellikle login endpoint’lerinde) ekle.
- [ ] Güvenlik testleri (authN/authZ, token misuse, privilege escalation) için otomasyon hazırla.

### 3) Domain ve İş Akışları
- [ ] Her servis için asgari CRUD dışı iş kuralı senaryolarını netleştir (ör. Stock rezervasyon, Sales sipariş akışı).
- [ ] Domain event’leri servis bazlı tanımlayıp event sözleşmelerini versiyonla.
- [ ] Validation kurallarını command seviyesinde genişlet (format, aralık, cross-field doğrulama).
- [ ] Idempotency gerektiren endpoint’ler için anahtar bazlı tekrar istek koruması ekle.
- [ ] Hata kodlarını ve API response şemasını servisler arası standartlaştır.

### 4) Veri Katmanı ve Migration Disiplini
- [ ] Tüm DbContext’lerde indeks/unique constraint gözden geçirme checklist’i oluştur.
- [ ] Seed stratejisini (zorunlu referans veriler) servis bazlı tanımla.
- [ ] Migration naming ve release süreci için konvansiyon dokümanı ekle.
- [ ] Transaction sınırlarını ve UnitOfWork kullanım yerlerini gözden geçirip netleştir.
- [ ] Soft delete / audit alanları için ortak politika ve uygulama denetimi ekle.

### 5) Test ve Kalite
- [ ] Her servis için `Application` katmanında unit test projesi aç.
- [ ] API seviyesinde integration test (test container veya localdb stratejisiyle) ekle.
- [ ] Sözleşme testleri (contract tests) ile servisler arası kırılmaları erken yakala.
- [ ] Minimum code coverage hedefi belirle ve CI’da threshold uygula.
- [ ] Lint/format/analyzer kurallarını pipeline’a zorunlu adım olarak ekle.

### 6) DevOps, CI/CD ve Operasyon
- [ ] CI pipeline’da build + test + güvenlik taraması (SAST/dependency scan) adımlarını zorunlu hale getir.
- [ ] CD sürecinde migration yönetimi ve rollback stratejisini dokümante et.
- [ ] Docker imajlarını servis bazlı optimize et (multi-stage build, küçük runtime image).
- [ ] Ortam bazlı health/readiness/liveness kontrollerini genişlet.
- [ ] Alerting ve SLO metriklerini belirleyip (hata oranı, latency, throughput) izlemeye al.

### 7) Dokümantasyon ve Geliştirici Deneyimi
- [ ] Her servis için kısa “how-to run locally” dokümanı standardize et.
- [ ] Swagger açıklamalarını ve örnek request/response içeriklerini zenginleştir.
- [ ] Mimarinin güncel diyagramlarını (servis sınırları + iletişim) ekle.
- [ ] “Definition of Done” checklist’i oluştur (test, log, güvenlik, dokümantasyon kriterleri).
- [ ] Yeni geliştirici onboarding adımlarını tek bir rehberde topla.
