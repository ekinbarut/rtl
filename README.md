**TVMazeScraper**
=============================

**Overview**
------------

**MazeScraper** is a .NET 9 solution that **scrapes TV show and cast data** from the [TVMaze API](https://www.tvmaze.com/api) and provides a **REST API** to expose that data. It consists of:

1.  **MazeScraper.Console**: A console app (scraper) that populates MongoDB with show/cast data.
2.  **MazeScraper.Api**: An ASP.NET Core Web API that **reads** from MongoDB and serves data in paginated, JSON format.
3.  **MazeScraper.Data**: A data-access layer (repositories, MongoDB context).
4.  **MazeScraper.Domain**: Domain models (`TvShow`, `Cast`, `Image`, etc.).
5.  **MazeScraper.Client** Extended HttpClient that calls the TVMaze API.
6.  **MazeScraper.Common** Common project with configurations.
7.  **MazeScraper.Tests**: Unit & integration tests for the scraper, repositories, services, and controllers.


**Getting Started**
-------------------

### **Prerequisites**

1.  [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet)
2.  [MongoDB](https://www.mongodb.com/) (locally or via Docker)
3.  (Optional) [Docker](https://www.docker.com/) for containerized deployment.

* * *

**Running the Project Locally (Without Docker)**
------------------------------------------------

1.  **Start MongoDB** (e.g., `mongod` locally or use a Docker container):
    
    ```bash
    docker-compose up
    ```
    
2.  **Configure** the connection string. In **`appsettings.json`** for both _MazeScraper.Api_ and _MazeScraper.Console_, set:
    
    ```json
    {
      "MongoDb": {
        "ConnectionString": "mongodb://localhost:27017",
        "DatabaseName": "MazeScraperDB"
      }
    }
    ```
    
3.  **Run the Scraper** (populates MongoDB):
    
    ```bash
    cd MazeScraper.Console
    dotnet run
    ```
    
    You may see logs indicating pages of shows fetched. The console app will exit when done.
    
4.  **Run the API**:
    
    ```bash
    cd ../MazeScraper.Api
    dotnet run
    ```
    
    By default, it listens on `http://localhost:5000`.
    
5.  **Test the API**:
    
    ```bash
    curl http://localhost:5000/api/tvshows?page=0&pageSize=5
    ```
    
    Should return a paginated list of shows with cast info.
    

* * *

**API Endpoints**
-----------------

### **GET** `/api/tvshows`

Returns a **paginated list** of shows, each with a **cast** array sorted by birthday descending.  
**Query Parameters**:

*   `page` (default `0`)
*   `pageSize` (default `10`)

**Sample Response**:

```json
[
  {
    "id": 1,
    "name": "Game of Thrones",
    "type": "Scripted",
    "cast": [
      { "id": 7, "name": "Actor A", "birthday": "1979-07-17" },
      { "id": 8, "name": "Actor B", "birthday": "1970-01-01" }
    ]
  },
  ...
]
```

* * *

**Scraper Usage**
-----------------

**MazeScraper.Console** fetches data from TVMaze and stores it in MongoDB.

*   It pulls pages of **shows** from `https://api.tvmaze.com/shows?page=N`.
*   Then fetches **casts** for each show from `https://api.tvmaze.com/shows/{id}/cast`.
*   Rate-limit or retry logic prevents hitting the 429 limit.
*   Stores results via **repositories** (`TvShowRepository`, `CastRepository`).


* * *

**Project Extensions**
----------------------

1.  **Incremental Scraping**:  
    Use `/updates/shows` from TVMaze to fetch only changed data.
2.  **Distributed Scraper**:  
    Use a queue-based approach if data is huge.
3.  **Authentication / Authorization**:  
    Secure the API with JWT or OAuth.
4.  **More Endpoints**:  
    Add show detail endpoints, advanced searching, etc.
5.  **Caching**:  
    Cache shows or cast queries to reduce DB load.

* * *
