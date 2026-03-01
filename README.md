### Table query (sql server)
create table Notes
(
    id         int identity
        primary key,
    title      varchar(100)       not null,
    content    varchar(max),
    created_at datetime default sysdatetime(),
    updated_at datetime,
    type       int      default 0 not null
)
go



### Myenvironment
-dotnet : 10.0.103
-vue : 3
-sql server : docker image (mcr.microsoft.com/mssql/server:2022-latest)
-style : tailwind
-ui lib : shadcn

### Feature 
Frontend
[x] CRUD Operations: Full implementation of Create, Read, Update, and Delete on the Notes list page.
[x] Data Management: Integrated simple filtering and sorting functionality.
[x] Search: Real-time search capabilities for note retrieval.
[x] UI/UX: Fully responsive design authored with Tailwind CSS.
[x] API Integration: Robust asynchronous data fetching using Axios/Fetch.
[x] State Management: Centralized application state handling.

Backend
[x] Note Services: Core CRUD logic for persistence layer.
[x] Data Access: High-performance mapping using Dapper ORM with SQL Server.
