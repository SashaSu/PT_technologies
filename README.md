# Product Catalog API

Тестовое задание: REST API для карточек товаров маркетплейса.

## Стек

- .NET 9, ASP.NET Core Web API
- PostgreSQL 16, Entity Framework Core
- FluentValidation
- SignalR для событий
- xUnit, Moq для тестов

## Запуск

Требования .NET 9 SDK и Docker.

```bash
docker compose up -d
dotnet run --project src/ProductCatalog.Api
```
Приложение стартует на `http://localhost:5021`


## Тесты

```bash
dotnet test
```

- unit-тесты сервиса (Moq), валидаторов и сервиса экспорта
- интеграционные тесты API 

## API

| Метод | Адрес | Описание |
|---|---|---|
| GET | `/api/products?page=1&pageSize=20` | список с пагинацией |
| GET | `/api/products/{id}` | один товар |
| POST | `/api/products` | создать |
| PUT | `/api/products/{id}` | изменить |
| DELETE | `/api/products/{id}` | удалить |
| POST | `/api/export` | экспорт всех товаров во внешний сервис |

## Валидация

Проверяется в `Validators/` (FluentValidation): обязательные `Name`, `Sku`, `Brand`; `Price` и `Weight` больше 0; `Quantity` не меньше 0; `Rating` от 0 до 5; enum только допустимые значения. Ошибки возвращаются как `400` со списком по полям. Дубликат артикула (`Sku`) даёт `409`.

## События
Используется SignalR
Посмотреть события можно на странице `http://localhost:5021/`

## Экспорт

`POST /api/export` отправляет товары на адрес из `Export:Url` (`appsettings.json`). Внешний сервис заменен мок-эндпоинтом `/mock/import` 

## Структура

```
src/ProductCatalog.Api/
  Controllers/   контроллеры
  Dtos/          модели запросов и ответов
  Data/          AppDbContext
  Repositories/  доступ к БД
  Services/      бизнес-логика, экспорт, события
  Validators/    правила валидации
tests/ProductCatalog.Tests/
```
