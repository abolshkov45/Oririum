@echo off
@if exist "%cd%\StartServer.bat" del /s /q %cd%\StartServer.bat
@if exist "%cd%\genesis.json" del /s /q %cd%\genesis.json
@if exist "%cd%\geth" rmdir /s /q %cd%\geth
@if exist "%cd%\keystore" rmdir /s /q %cd%\keystore
@echo 1. Создание первичного аккаунта.
@echo Введите пароль и подтвердите его.
cmd /c "geth --datadir="%cd%" account new"
@echo 2. Генерация genesis блока.
set /P chainId="Введите адрес сети: "
set /P accountAddress="Введите адрес созданного аккаунта он находится в строке "Public address of the key:": "
set /P accountBalance="Введите стартовое количество монет для аккаунта: "
@echo { > genesis.json
@echo  "config": { >> genesis.json
@echo    "chainId": %chainId%, >> genesis.json
@echo    "homesteadBlock": 0, >> genesis.json
@echo    "eip155Block": 0, >> genesis.json
@echo    "eip158Block": 0, >> genesis.json
@echo    "eip150Block": 0 >> genesis.json
@echo  }, >> genesis.json
@echo  "alloc": { >> genesis.json
@echo    "%accountAddress%": { >> genesis.json
@echo      "balance": "%accountBalance%" >> genesis.json
@echo    } >> genesis.json
@echo  }, >> genesis.json
@echo  "difficulty": "0x20000", >> genesis.json
@echo  "extraData": "", >> genesis.json
@echo  "gasLimit": "0x2fefd8", >> genesis.json
@echo  "nonce": "0x0000000000000042", >> genesis.json
@echo  "mixhash": "0x0000000000000000000000000000000000000000000000000000000000000000", >> genesis.json
@echo  "parentHash": "0x0000000000000000000000000000000000000000000000000000000000000000", >> genesis.json
@echo  "timestamp": "0x00" >> genesis.json
@echo } >> genesis.json
cmd /c "geth --datadir="%cd%" init %cd%\genesis.json"
@echo 3. Запуск сервера.
set /P ip="Введите IP-адрес сервера: "
set /P port="Введите порт сервера: "
@echo @echo off > StartServer.bat
@echo @echo Сервер запускается... >> StartServer.bat
@echo cmd /c "geth --datadir="%cd%" --networkid %chainId% --nodiscover --miner.gasprice 0 --miner.gastarget 470000000000 --rpc --rpcport "%port%" --rpcaddr "%ip%" --rpccorsdomain "*" --rpcapi "eth,net,web3,miner,debug,personal,rpc" --preload "%cd%\noMine.js" --allow-insecure-unlock" >> StartServer.bat
@echo Настройка завершена. Файл StartServer.bat успешно создан!
@pause