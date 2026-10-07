# 📚 Biblioteca.Web — ASP.NET Core MVC con Dapper

> **Laboratorio Semana 08 · MVC con Dapper**
> **Curso:** Desarrollo de Aplicaciones Empresariales Avanzadas — Tecsup
> **Docente:** Arévalo Sermeño, Edwin William
> **Autor:** Vidal Hotaru Azañero Pillaca

Aplicación web de gestión de biblioteca construida con **ASP.NET Core MVC**, que accede a la base de datos **BibliotecaDB** (SQL Server) mediante **Dapper** y procedimientos almacenados. Todo el código vive en un único proyecto, sin referenciar proyectos de semanas anteriores.

---

## 🛠️ Tecnologías

| Componente | Detalle |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Acceso a datos | Dapper 2.1.35 |
| Driver SQL | Microsoft.Data.SqlClient 5.2.2 |
| Base de datos | SQL Server (`BibliotecaDB`) |
| Vistas | Razor + Bootstrap 5 |

---

## 🏗️ Estructura del proyecto

```
Biblioteca.Web/
├── Controllers/
│   ├── LibrosController.cs        # Index, Details, Create, Edit, Delete
│   ├── SociosController.cs        # Index, Create
│   └── PrestamosController.cs     # Reporte
├── Models/
│   ├── Libro.cs                   # Con DataAnnotations
│   ├── Autor.cs
│   ├── Socio.cs
│   └── PrestamoReporte.cs
├── Repositorios/
│   ├── ILibroRepositorio.cs / LibroRepositorio.cs
│   ├── ISocioRepositorio.cs / SocioRepositorio.cs
│   └── IPrestamoRepositorio.cs / PrestamoRepositorio.cs
├── Views/
│   ├── Libros/                    # Index, Details, Create, Edit, Delete + parciales
│   ├── Socios/                    # Index, Create
│   ├── Prestamos/                 # Reporte
│   └── Shared/_Layout.cshtml      # Menú de navegación
├── appsettings.json               # Cadena de conexión
└── Program.cs                     # Registro de repositorios (DI)
```

### Flujo de una petición

```
Navegador → Ruta → Controlador → Repositorio (Dapper) → Procedimiento almacenado → Vista Razor
```

Los controladores **no contienen SQL ni abren conexiones**: solo reciben los repositorios por constructor (inyección de dependencias) y devuelven vistas.

---

## ✨ Funcionalidades

- **📚 Libros:** listado con búsqueda, detalle, registro, edición y eliminación **lógica** (`Activo = 0`). Los formularios incluyen una lista desplegable de autores y validaciones con DataAnnotations (`[Required]`, `[StringLength]`, `[Range]`, `[DataType]`). La fila de cada libro es una **vista parcial** reutilizada en el listado y en los resultados de búsqueda.
- **👥 Socios:** listado y registro. Si el DNI ya existe, el formulario muestra un mensaje de validación (`ModelState.AddModelError`).
- **📊 Reporte de préstamos:** filtro por rango de fechas (`desde` y `hasta`, enviados por GET) que muestra socio, libros, fecha límite y estado. Usa un `INNER JOIN` entre `Prestamos`, `DetallePrestamo`, `Libros` y `Socios`.
- **Patrón Post/Redirect/Get:** tras guardar, editar o eliminar se redirige y el resultado se muestra con `TempData`.
- **Async de punta a punta:** todas las acciones son `async Task<IActionResult>`, sin `.Result` ni `.Wait()`.

---

## ⚙️ Configuración y ejecución

### 1. Requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (el proyecto apunta a `net10.0`)
- SQL Server con la base de datos **BibliotecaDB** ya creada (tablas, datos de prueba y procedimientos almacenados de la semana anterior)

### 2. Clonar el repositorio

```bash
git clone <URL-DEL-REPOSITORIO>
cd Biblioteca.Web
```

### 3. Cadena de conexión

Edita `appsettings.json` con tu servidor:

```json
"ConnectionStrings": {
  "BibliotecaDB": "Server=TU_SERVIDOR;Database=BibliotecaDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Si usas autenticación de SQL Server, reemplaza `Trusted_Connection=True` por `User Id=...;Password=...`.

### 4. Ejecutar

```bash
dotnet restore
dotnet run
```

La aplicación queda disponible en la dirección que indique la consola (por ejemplo, `http://localhost:5075`).

---

## 🗄️ Base de datos

Se **reutiliza** la base de datos `BibliotecaDB` de la semana anterior; no se crea una nueva. La aplicación consume estos procedimientos almacenados:

| Módulo | Procedimientos |
|---|---|
| Libros | `sp_Libros_Listar`, `sp_Libros_Buscar`, `sp_Libros_ObtenerPorId`, `sp_Libros_Insertar`, `sp_Libros_Actualizar`, `sp_Libros_EliminarLogico`, `sp_Libros_ExisteISBN`, `sp_Libros_TienePrestamosPendientes` |
| Autores | `sp_Autores_Listar` |
| Socios | `sp_Socios_Listar`, `sp_Socios_Insertar`, `sp_Socios_ExisteDNI` |
| Préstamos | `sp_Prestamos_ReportePorFechas` |

---

## 🖼️ Capturas de las vistas

> Agrega aquí las capturas (por ejemplo, en una carpeta `capturas/`):

| Vista | Captura |
|---|---|
| Listado de libros | `![Libros](capturas/libros-index.png)` |
| Crear / editar libro | `![Crear libro](capturas/libros-create.png)` |
| Listado de socios | `![Socios](capturas/socios-index.png)` |
| Validación de DNI duplicado | `![DNI duplicado](capturas/socios-dni.png)` |
| Reporte de préstamos | `![Reporte](capturas/prestamos-reporte.png)` |

---

## 📝 Explicación: recorrido de una petición (`Libros/Create`)

1. **Ruta:** el navegador solicita `GET /Libros/Create`. El enrutamiento por convención (`{controller}/{action}/{id?}`, definido en `Program.cs`) lo asigna a `LibrosController.Create`.
2. **Controlador:** la acción GET carga la lista de autores con `CargarAutoresAsync` y devuelve la vista con un `Libro` nuevo.
3. **Repositorio:** `LibroRepositorio.ListarAutoresAsync` ejecuta `sp_Autores_Listar` con Dapper (`QueryAsync` + `CommandType.StoredProcedure`).
4. **Vista:** `Views/Libros/Create.cshtml` (tipada con `@model Libro`) muestra el formulario con la lista desplegable.
5. **Envío (POST):** al guardar, `Create(Libro libro)` valida con `ModelState.IsValid`, comprueba que el ISBN no exista (`sp_Libros_ExisteISBN`) e inserta con `sp_Libros_Insertar`.
6. **Redirección:** se redirige a `Index` (Post/Redirect/Get) y el mensaje de éxito se muestra con `TempData`.

**¿Qué se usa para pasar datos a la vista y por qué?**
- **Modelo (`@model Libro`):** para los datos principales del formulario; es fuertemente tipado y activa la validación de DataAnnotations.
- **`ViewData`:** para datos auxiliares que no pertenecen al modelo, como el título de la página y la lista de autores.
- **`TempData`:** para el mensaje de resultado, porque sobrevive a la redirección; `ViewData` se perdería al cambiar de petición.

---

## 💡 Observaciones y conclusiones

- Separar el acceso a datos en repositorios deja los controladores sin SQL y facilita cambiar la fuente de datos.
- Dapper reduce el código repetitivo de ADO.NET manteniendo el uso de procedimientos almacenados y parámetros, lo que previene la inyección SQL.
- La cadena de conexión vive en `appsettings.json` y se lee con `IConfiguration`, sin escribirla en el código.
- Las validaciones con DataAnnotations se aplican tanto en el servidor como en el cliente, y se complementan con reglas de negocio (ISBN y DNI únicos).
