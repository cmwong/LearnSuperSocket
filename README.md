# LearnSuperSocket
Learn [SuperSocket](https://github.com/kerryjiang/SuperSocket) and [SuperSocket.ClientEngine](https://github.com/kerryjiang/SuperSocket.ClientEngine)

### BasicClientServer01
* extend EasyClient
* Client Command

### BasicClientServer02
* change transmit string to json (Command RequestAdd, ResponseAdd)
* sync RequestAdd method

### BasicClientServer03
*  hide the supersocket client in our own client class. ("has a" supersocket client)

### BasicClientServer03.net35
* i want client in .net 3.5

### WebSocket01
* very basic WebSocket testing

### WebSocket02
* Server: extend JsonWebSocketSession, override JsonSerialize, JsonDeserialize to use Json.Net
* Client: extend JsonWebSocket, override SerializeObject, DeserializeObject to use Json.Net

### SocketServer
* Implement custom FixedHeader protocol on both server and client
###### FixedHeader 8 bytes into 4 uint16
###### 1st value = ?
###### 2nd value = body length
###### 3rd value = mainKey
###### 4th value = subKey
