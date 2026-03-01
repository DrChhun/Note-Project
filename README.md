### Table query (SQL Server)

```sql
create table Notes
(
    id         int identity primary key,
    title      varchar(100) not null,
    content    varchar(max),
    created_at datetime default sysdatetime(),
    updated_at datetime,
    type       int default 0 not null
)
go
```

My Environment :

dotnet: 10.0.103
vue: 3
sql server: docker image (mcr.microsoft.com/mssql/server:2022-latest)
style: tailwind
ui lib: shadcn

Feature :
 + Frontend
 CRUD Operations: Full implementation of Create, Read, Update, and Delete.
 Data Management: Filtering and sorting.
 Search: Real-time search.
 UI/UX: Responsive design with Tailwind CSS.
 API Integration: Axios/Fetch.
 State Management: Centralized state handling.
 
 + Backend
 Note Services: Core CRUD logic.
 Data Access: Dapper ORM with SQL Server.
