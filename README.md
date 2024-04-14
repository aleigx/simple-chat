### Ejecutando la Aplicación

Para descargar el cliente Docker, utiliza el siguiente comando:

```bash
docker pull aleigx/simple-chat-client
```

Para descargar el servidor Docker, utiliza el siguiente comando:

```bash
docker pull aleigx/simple-chat-server
```

Crea una red Docker para la aplicación de chat:

```bash
docker network create chat-app
```

Inicia el contenedor del servidor de chat:

```bash
docker run -d -p 9898:9898 --network chat-app --name chat-server-container aleigx/simple-chat-server
```

Inicia el primer contenedor del cliente de chat:

```bash
docker run -d -p 22:22 --network chat-app --name chat-client-container aleigx/simple-chat-client /usr/sbin/sshd -D
```

Inicia el segundo contenedor del cliente de chat:

```bash
docker run -d -p 23:22 --network chat-app --name chat-client-container2 aleigx/simple-chat-client /usr/sbin/sshd -D
```

### Probando la Aplicación

Para probar la aplicación, conecta al primer cliente usando el siguiente comando:

```bash
ssh root@localhost
```

Para conectarte al segundo cliente, usa:

```bash
ssh root@localhost -p 23
```

Si encuentras el error "REMOTE HOST IDENTIFICATION HAS CHANGED!", ejecuta el siguiente comando:

```bash
ssh-keygen -R localhost
```

Para cada cliente, inicia sesión usando:

```bash
"chat chat-server-container usuario"
```

Asegúrate de que el nombre de usuario sea único para evitar errores.