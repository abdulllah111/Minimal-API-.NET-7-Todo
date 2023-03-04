
# Minimal-API-.NET-7-Todo

Проект разработан на .Net7



## Что было разработано

- База данных на PostreSql
- CRUD
- Интегрирован Swagger
- Bearer аунтефикация
- Нормализована декомпозиция


## FAQ

### Во первых:

- Настраиваем строку подключения к базе данных. Найти её можно в файле *appsettings.json* строка *"DefaultConnection"*

### Во вторых:

- Создаём миграцию.
```bash
dotnet ef migrations add newmigration --project Abdul.ToDo.csproj
```

### В третьих:

- Запуск:
```bash
dotnet run
 ```
 Видим ссылку http://localhost:5071 - Копируем

#### Вариант запуска через swagger:
- В браузере вставляем нашу ссылку и дописываем к ней /swagger/index.html 
- Должно получиться так:  http://localhost:5071/swagger/index.html

#### Вариант запуска через httprepl:
- Устанавка
```bash
dotnet tool install -g Microsoft.dotnet-httprepl
 ```
 - Запуск
```bash
httprepl  http://localhost:5071
 ```
 После запуска мы можем вводить запросы (get, post...) в терминал.
 
 </br>
 </br>
 
 
 
![](https://komarev.com/ghpvc/?username=abdulllah111&color=green&label=Посещений )
