# Linux systemd 安裝說明

## 1. 發佈

### framework-dependent

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r linux-x64 --self-contained false -o publish/linux
```

### self-contained

```bash
dotnet publish src/MySelfHostService/MySelfHostService.csproj -c Release -r linux-x64 --self-contained true -o publish/linux
```

## 2. 複製檔案

假設部署到：

```bash
/opt/myselfhostservice
```

```bash
sudo mkdir -p /opt/myselfhostservice
sudo cp -r ./publish/linux/* /opt/myselfhostservice/
```

若為 self-contained：

```bash
sudo chmod +x /opt/myselfhostservice/MySelfHostService
```

## 3. 建立 systemd 服務檔

建立：

```bash
sudo nano /etc/systemd/system/myselfhostservice.service
```

### self-contained

```ini
[Unit]
Description=My .NET Self Host Service
After=network.target

[Service]
WorkingDirectory=/opt/myselfhostservice
ExecStart=/opt/myselfhostservice/MySelfHostService
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=myselfhostservice
User=www-data
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=DOTNET_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

### framework-dependent

```ini
[Unit]
Description=My .NET Self Host Service
After=network.target

[Service]
WorkingDirectory=/opt/myselfhostservice
ExecStart=/usr/bin/dotnet /opt/myselfhostservice/MySelfHostService.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=myselfhostservice
User=www-data
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=DOTNET_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

## 4. 啟動與管理

```bash
sudo systemctl daemon-reload
sudo systemctl enable myselfhostservice
sudo systemctl start myselfhostservice
```

查看狀態：

```bash
sudo systemctl status myselfhostservice
```

查看 log：

```bash
sudo journalctl -u myselfhostservice -f
```

停止：

```bash
sudo systemctl stop myselfhostservice
```

重啟：

```bash
sudo systemctl restart myselfhostservice
```

## 5. 防火牆

如果有啟用 `ufw`：

```bash
sudo ufw allow 5000/tcp
```
