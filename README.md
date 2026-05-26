# .NET Cross-Platform HTTP Self-Host Service Sample

這個範例示範如何建立一個跨平台的 .NET HTTP Self-host 服務，使用 **ASP.NET Core Minimal API + Generic Host**，並支援：

- 靜態檔案：`txt`、`html`
- 背景工作：定時寫 log
- 可設定 Port 與服務名稱的 `appsettings.json`
- Serilog Console + File logging
- Windows：安裝成 Windows Service
- Linux：安裝成 systemd 背景服務
- POST JSON API 範例

此專案可直接以命令列執行，也可安裝成背景服務。ASP.NET Core 可用 `UseWindowsService()` 與 `UseSystemd()` 整合 Windows Service 與 systemd。Microsoft 的 Windows Service 文件也特別提到可用 `ASPNETCORE_URLS` 設定服務的 URL/Port。 citeturn0search1turn0search3

## 專案結構

```text
.
├─ README.md
├─ deploy/
│  ├─ linux-systemd.md
│  └─ windows-service.md
└─ src/
   └─ MySelfHostService/
      ├─ appsettings.json
      ├─ MySelfHostService.csproj
      ├─ Program.cs
      └─ wwwroot/
         ├─ index.html
         ├─ test.txt
         └─ sub/
            └─ hello.html
```

## 功能

- `GET /`：回應 `wwwroot/index.html`
- `GET /test.txt`：回應文字檔
- `GET /sub/hello.html`：回應子目錄中的 HTML
- `GET /health`：回應健康檢查 JSON
- `POST /api/echo`：回應送入的 JSON 與伺服器時間
- 背景服務每 30 秒寫一筆 log
- Serilog 將 log 輸出到 console 與每日 rolling file

Serilog.AspNetCore 官方 README 建議在 ASP.NET Core 中使用 `UseSerilog(...)` 來整合主機記錄；Serilog file sink 支援 `WriteTo.File()` 與 JSON `appsettings.json` 設定，並支援每日 rolling file。 citeturn0search4turn0search0turn0search2

## 需求

- .NET 8 SDK（開發）
- Windows 或 Linux
- 若採 framework-dependent 發佈，目標機器需安裝對應 .NET Runtime

## 本機執行

```bash
dotnet restore src/MySelfHostService/MySelfHostService.csproj
dotnet run --project src/MySelfHostService/MySelfHostService.csproj
```

預設監聽：

- `http://localhost:5000/`
- `http://localhost:5000/test.txt`
- `http://localhost:5000/sub/hello.html`
- `http://localhost:5000/health`
- `http://localhost:5000/api/echo`

### 測試 GET API

```bash
curl http://localhost:5000/health
```

### 測試 POST API

```bash
curl -X POST http://localhost:5000/api/echo \
  -H "Content-Type: application/json" \
  -d '{"message":"hello","from":"client"}'
```

範例回應：

```json
{
  "received": {
    "message": "hello",
    "from": "client"
  },
  "serverTime": "2026-05-26T00:00:00+00:00"
}
```

## 設定

程式從 `appsettings.json` 讀取：

- `ServiceOptions:Port`
- `ServiceOptions:Name`
- `Serilog` logging 設定

也可以在部署環境用環境變數覆蓋，例如：

```bash
ASPNETCORE_URLS=http://0.0.0.0:5000
DOTNET_ENVIRONMENT=Production
```

Microsoft Learn 說明，Windows Service 情境可透過 `ASPNETCORE_URLS` 設定 URL/Port。 citeturn0search3

## 發佈

### Windows

framework-dependent：

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r win-x64 --self-contained false -o publish/win
```

self-contained：

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r win-x64 --self-contained true -o publish/win
```

### Linux

framework-dependent：

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r linux-x64 --self-contained false -o publish/linux
```

self-contained：

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r linux-x64 --self-contained true -o publish/linux
```

## Serilog

此範例將 log 輸出到：

- Console
- `logs/log-.txt`（每天一個檔案）

Serilog file sink 官方文件指出，可用 `rollingInterval: Day` 建立每日 rolling log，且可用 JSON 設定方式配置 File sink。 citeturn0search0turn0search4

## 背景服務安裝

請參考：

- `deploy/windows-service.md`
- `deploy/linux-systemd.md`

## 後續可延伸

- 加入 HTTPS / Reverse Proxy
- 加入驗證與授權
- 將 API 拆成 Controllers
- 加入 Swagger/OpenAPI
- 加入 GitHub Actions 自動發佈
