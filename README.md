# .NET Cross-Platform HTTP Self-Host Service Sample

這個範例示範如何建立一個跨平台的 .NET HTTP Self-host 服務，支援：

- 靜態檔案：`txt`、`html`
- 背景工作：定時寫 log
- Windows：安裝成 Windows Service
- Linux：安裝成 systemd 背景服務

## 專案結構

```text
.
├─ README.md
├─ src/
│  └─ MySelfHostService/
│     ├─ MySelfHostService.csproj
│     ├─ Program.cs
│     └─ wwwroot/
│        ├─ index.html
│        ├─ test.txt
│        └─ sub/
│           └─ hello.html
└─ deploy/
   ├─ windows-service.md
   └─ linux-systemd.md
```

## 建立與執行

### 建立

```bash
dotnet restore
```

### 本機執行

```bash
dotnet run --project src/MySelfHostService/MySelfHostService.csproj
```

預設監聽：

- `http://localhost:5000/`
- `http://localhost:5000/test.txt`
- `http://localhost:5000/sub/hello.html`
- `http://localhost:5000/health`

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

## 功能說明

- `/`：回應 `wwwroot/index.html`
- `/test.txt`：回應文字檔
- `/sub/hello.html`：回應子目錄中的 HTML
- `/health`：回應健康檢查 JSON
- 背景服務每 30 秒寫一筆 log

## Windows / Linux 背景服務安裝

請參考：

- `deploy/windows-service.md`
- `deploy/linux-systemd.md`
