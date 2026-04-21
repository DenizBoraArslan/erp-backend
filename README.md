# erp-backend

Bu repository’de her servis kendi veritabanı ve migration yapısına sahiptir.

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

> Not: Çözümde şu an ayrı bir otomatik test projesi bulunmadığı için aşağıdaki adımlar smoke test / manuel doğrulama içindir.

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
   - API açılış loglarında hata olmadığını doğrula.
   - Swagger/OpenAPI ekranının açıldığını doğrula.
   - En az bir endpoint’e istek atıp başarılı yanıt alındığını kontrol et.

5. **Veritabanı kontrolü**
   - İlk açılışta migration’ların otomatik uygulandığını loglardan doğrula.
   - Gerekirse `dotnet ef database update` ile manuel güncelleme yap ve servisi tekrar başlatıp endpoint’leri tekrar test et.
