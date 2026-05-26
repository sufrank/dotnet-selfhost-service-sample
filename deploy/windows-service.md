# Windows Service 安裝說明

## 1. 發佈

### framework-dependent

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r win-x64 --self-contained false -o publish/win
```

### self-contained

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r win-x64 --self-contained true -o publish/win
```

## 2. 複製檔案

將 `publish/win` 內容複製到例如：

```text
C:\Services\MySelfHostService
```

## 3. 建立服務

### self-contained

```cmd
sc create MySelfHostService binPath= "C:\Services\MySelfHostService\MySelfHostService.exe" start= auto
```

### framework-dependent

```cmd
sc create MySelfHostService binPath= "dotnet C:\Services\MySelfHostService\MySelfHostService.dll" start= auto
```

## 4. 管理服務

啟動：

```cmd
sc start MySelfHostService
```

查詢：

```cmd
sc query MySelfHostService
```

停止：

```cmd
sc stop MySelfHostService
```

刪除：

```cmd
sc delete MySelfHostService
```

## 5. 防火牆

PowerShell：

```powershell
New-NetFirewallRule -DisplayName "MySelfHostService 5000" `
  -Direction Inbound `
  -Protocol TCP `
  -LocalPort 5000 `
  -Action Allow
```
