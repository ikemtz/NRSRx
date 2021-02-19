IkeMtz's DB, EF and Microservices Best Practices

- [Database Best Practices](#database-best-practices)
  - [Table names should be plural](#table-names-should-be-plural)
  - [Table and column names should be in Pascal case](#table-and-column-names-should-be-in-pascal-case)
  - [Table should have a primary key (🔑PK) named Id](#table-should-have-a-primary-key-pk-named-id)
  - [The chosen primary key data type should depend on a variety of factors](#the-chosen-primary-key-data-type-should-depend-on-a-variety-of-factors)
  - [Foreign key column names should follow the {ParentKeyTableName}+'Id' naming convention](#foreign-key-column-names-should-follow-the-parentkeytablenameid-naming-convention)
  - [Column names should not repeat table names](#column-names-should-not-repeat-table-names)
  - [Child column names should not repeat table names](#child-column-names-should-not-repeat-table-names)
  - [Apply a standard list of abbreviations on column names](#apply-a-standard-list-of-abbreviations-on-column-names)
    - [**Suggested Standards**](#suggested-standards)
  - [DateTime values should be stored in UTC and the column name should be suffixed with '**Utc**'](#datetime-values-should-be-stored-in-utc-and-the-column-name-should-be-suffixed-with-utc)
  - [Date (NOT DateTime) columns should be suffixed with '**Date**'](#date-not-datetime-columns-should-be-suffixed-with-date)
  - [Performance > Normalization](#performance--normalization)
- [Entity Framework Best Practices](#entity-framework-best-practices)
  - [Entity names should be singular and DbSets should be plural](#entity-names-should-be-singular-and-dbsets-should-be-plural)
  - [Entity models should have an Id property that reflects a unique value](#entity-models-should-have-an-id-property-that-reflects-a-unique-value)
  - [Entity models should properly define navigation properties](#entity-models-should-properly-define-navigation-properties)
  - [Entity models representing volatile data should support audit fields](#entity-models-representing-volatile-data-should-support-audit-fields)
- [Microservice Best Practices](#microservice-best-practices)
  - [Microservices should wrap a single business domain and all of it's logic](#microservices-should-wrap-a-single-business-domain-and-all-of-its-logic)
  - [Microservice naming](#microservice-naming)
  - [Microservices should have well documented APIs](#microservices-should-have-well-documented-apis)
  - [Microservices should be built around the technology best suited to meet business needs.](#microservices-should-be-built-around-the-technology-best-suited-to-meet-business-needs)
  - [Eventing and inter-microservice dependencies](#eventing-and-inter-microservice-dependencies)
  - [Microservices should validate each piece of input data](#microservices-should-validate-each-piece-of-input-data)
  - [Each Microservice endpoint should require authentication](#each-microservice-endpoint-should-require-authentication)
  - [Service and API Versionining](#service-and-api-versionining)

# Database Best Practices
## Table names should be plural

When applying this rule, it's important to <b>NOT</b> adhere to normal language rules.  As an English example, you should use Persons instead of People.

**Don't**:

```SQL
CREATE TABLE Person { ... }
```

**Do**:

```SQL
CREATE TABLE Persons { ... }
```

## Table and column names should be in Pascal case
**Don't**:

```SQL
CREATE TABLE persons {
  [ID] UNIQUEIDENTIFIER NOT NULL,
  [name] NVARCHAR (50) NOT NULL,
  [birthDate] DATE NOT NULL }
```

**Do**:

```SQL
CREATE TABLE Persons {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [Name] NVARCHAR (50) NOT NULL,
  [BirthDate] DATE NOT NULL }
```

## Table should have a primary key (🔑PK) named Id
The importance of this may not be obvious, but this practice allows for commonality that can be leveraged in interfaces.

It's also important to note, that in a many-to-many relationship table, separate unique indexes should be used as necessary.

**Don't**:

```SQL
CREATE TABLE Persons {
  [PersonId] UNIQUEIDENTIFIER NOT NULL,
  ...,
  CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED ([PersonId] ASC)
   }
```

**Do**:

```SQL
CREATE TABLE Persons {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [Name] NVARCHAR (50) NOT NULL,
  CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED ([Id] ASC)
   }
```
## The chosen primary key data type should depend on a variety of factors
When considering data types for the primary key, the following factors must be taken into consideration:

1. Analyze the volatility of the data.  Is this a dataset that will change once a year?  Or will this set by managed by end users?
2. Some database technologies don't handle indexing of uniqueidentifiers well.
3. Will the schema be deployed to multiple of environments?  Do you need to ensure that data from one environment doesn't cross into another?

**Don't**:

```SQL
--Person type is a list that will change infrequently
--(Parent, Teacher, Student, etc.)
CREATE TABLE PersonTypes { 
  [Id] UNIQUEIDENTIFIER NOT NULL,
   }
--Persons will potentially change every day
CREATE TABLE Persons { 
  [Id] VARCHAR(5) NOT NULL,
   }
```

**Do**:
```SQL
CREATE TABLE PersonTypes { 
  [Id] VARCHAR(10) NOT NULL, -- OR INT
   }
CREATE TABLE Persons { 
  [Id] UNIQUEIDENTIFIER NOT NULL,
   }
```

## Foreign key column names should follow the {ParentKeyTableName}+'Id' naming convention

**Don't**:

```SQL
CREATE TABLE Orders {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  ...,
  CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
   }

CREATE TABLE OrderLineItems {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [Order] UNIQUEIDENTIFIER NOT NULL,
  ...,
  CONSTRAINT [PK_OrderLineItems] PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_OrderLineItems_Orders] FOREIGN KEY ([Order])
    REFERENCES [dbo].[Orders] ([Id])
   }
```

**Do**:

```SQL
CREATE TABLE Orders {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  ...,
  CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
   }

CREATE TABLE OrderLineItems {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [OrderId] UNIQUEIDENTIFIER NOT NULL,
  ...,
  CONSTRAINT [PK_OrderLineItems] PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_OrderLineItems_Orders] FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[Orders] ([Id])
   }
```



## Column names should not repeat table names

**Don't**:

```SQL
CREATE TABLE Persons {
  [PersonId] UNIQUEIDENTIFIER NOT NULL,
  [PersonName] NVARCHAR (50) NOT NULL,
  [PersonBirthDate] DATE NOT NULL }
```

**Do**:

```SQL
CREATE TABLE Persons {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [Name] NVARCHAR (50) NOT NULL,
  [BirthDate] DATE NOT NULL }
```

## Child column names should not repeat table names

**Don't**:

```SQL
CREATE TABLE Persons {
  [PersonId] UNIQUEIDENTIFIER NOT NULL,
  [PersonName] NVARCHAR (50) NOT NULL,
  [PersonBirthDate] DATE NOT NULL }
```

**Do**:

```SQL
CREATE TABLE Persons {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [Name] NVARCHAR (50) NOT NULL,
  [BirthDate] DATE NOT NULL }
```

## Apply a standard list of abbreviations on column names
When applying this rule, it's absolutely important to avoid ambiguity.  People should know immediately when the column name is referring to.

### **Suggested Standards**
| Word(s)            | Abbreviation |
| ------------------ | ------------ |
| Number             | Num          |
| Quantity           | Qty          |
| Identifier         | Id           |
| Customer           | Cust         |
| Telephone Number 1 | Tel 1        |
| Territory          | Terr         |
| Sequence           | Seq          |
| First Name         | FName        |
| Last Name          | LName        |
| Zip Code           | Zip          |

**Don't**:

```SQL
CREATE TABLE Orders {
  [Number] UNIQUEIDENTIFIER NOT NULL,
  [CustomerNumber] UNIQUEIDENTIFIER NOT NULL,
  [ItemQuantity] INT NOT NULL }
```

**Do**:

```SQL
CREATE TABLE Orders {
  [Num] UNIQUEIDENTIFIER NOT NULL,
  [CustNum] UNIQUEIDENTIFIER NOT NULL,
  [ItemQty] INT NOT NULL }
```

## DateTime values should be stored in UTC and the column name should be suffixed with '**Utc**'
**Don't**:

```SQL
CREATE TABLE Persons { ...,
  [BirthDate] DATE NOT NULL,
  [LastVisit] DATETIME NOT NULL }
```

**Do**:

```SQL
CREATE TABLE Persons { ...,
  [BirthDate] DATE NOT NULL,
  [LastVisitUtc] DATETIME NOT NULL }
```

## Date (NOT DateTime) columns should be suffixed with '**Date**'
This is important because serialization languages such as XML and JSON make no distinction between Date and DateTime.  This will provide microservice consumers an indication as to what values to expect.

**Don't**:

```SQL
CREATE TABLE Persons { ...,
  [DoB] DATE NOT NULL }
```

**Do**:

```SQL
CREATE TABLE Persons { ...,
  [BirthDate] DATE NOT NULL }
```

## Performance > Normalization
Rather than calculating values over and over, it's best to persist the calculated values.  For obvious reasons we don't want store every possible calculation, however, we do want to persist the more commonly requested calculations.  Particularly calculations that are part of a report or user interface.
**Don't**:
```SQL
CREATE TABLE Orders {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [ItemQty] INT NOT NULL,
  [Total] MONEY NOT NULL }
SELECT 
  Id, 
  ItemQty, 
  Total, 
  Total / Item AS AvgItemPrice
FROM Orders
```

**Do**:

```SQL
CREATE TABLE Orders {
  [Id] UNIQUEIDENTIFIER NOT NULL,
  [ItemQty] INT NOT NULL,
  [Total] MONEY NOT NULL,
  [AvgItemPrice] MONEY NOT NULL }
SELECT 
  Id, 
  ItemQty, 
  Total, 
  AvgItemPrice --I/O related to this calculation is now avoided
FROM Orders
```

# Entity Framework Best Practices
## Entity names should be singular and DbSets should be plural
it's important to <b>NOT</b> adhere to normal language rules.  As an English example, you should name the DbSet Persons instead of People.

**Don't**:

```C#
public class ApplicationEntities : DbContext
{
    public DbSet<Person> People { get; set; }
}
```

**Do**:

```C#
public class ApplicationEntities : DbContext
{
    public DbSet<Person> Persons { get; set; }
}
```
## Entity models should have an Id property that reflects a unique value 
This allows the creation of an IIdentifiable.  This is also a requirement for OData services.  Additionally, this is also supported by out of the box EF conventions.

**Don't**:

```C#
public class Person
{
    public Guid PersonId { get; set; }
}
```

**Do**:

```C#
public class Person : IIdentifiable
{
    public Guid Id { get; set; }
}
```

## Entity models should properly define navigation properties
This rule plays a significant role in the functionality provided by OData services.  The only exception to this rule is self-referencing entities, which are not supported by OData.

**Don't**:

```C#
public class Order : IIdentifiable
{
    public Guid Id { get; set; }    
    public Guid ParentOrderId { get; set; }
    // No self-referencing entities
    public virtual Order ParentOrder { get; set; } 
    // No self-referencing entities
    public virtual ICollection<Order> ChildOrders { get; set; } 
}
public class OrderLineItem : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
}
```

**Do**:
```C#
public class Order : IIdentifiable
{
    public Guid Id { get; set; }
    public virtual Guid ParentOrderId { get; set; }
    public virtual ICollection<OrderLineItem> OrderLineItems { get; set; }
}
public class OrderLineItem : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid OrderId {get; set;}
    public virtual Order Order { get; set; }
}
```

## Entity models representing volatile data should support audit fields
Depending on the environment and security requirements, auditable fields may be required to keep track of record changes.

Much of this functionality is streamlined by the IAuditable interface in the NRSRx framework.

**Do**:
```C#
public class Person : IIdentifiable, IIAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedOnUtc { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedOnUtc { get; set; }
} 
```

# Microservice Best Practices
## Microservices should wrap a single business domain and all of it's logic
Microservices should focus on a single domain.  Encompassing all of the business logic for said domain.

## Microservice naming
Ambiguitity in a distributed architecture of any kind would inevitably lead to confusion, errors and ultimately loss of productivity.

Microservice names should clearly reflect the domain being supported.  Microservice names and endpoints should be plural and in Pascal case.

## Microservices should have well documented APIs
When it comes to API documentation, the OpenAPI spec has become the defacto standard.  Leveraging the OpenAPI documents, the Swagger client side application is quite powerful and making these documents human parseable.

Ensuring that these components are available to developers will be crucial to facilitating cross team integration and success.

It's also important that the Swagger application be configured to support the same authentication mechanisms as the API.

## Microservices should be built around the technology best suited to meet business needs.
If you need to share data, then OData is an excellent choice.  If you need to persist domain objects and state, then WebApi would be the appropriate direction.

## Eventing and inter-microservice dependencies
At all costs, avoid having one microservice be dependent on another.  Each microservice should maintain a copy of any cross-domain data necessary in order to complete its work.  Updates to this cross-domain data should be communicated via an event bus.

## Microservices should validate each piece of input data
Each property of input data should be validated, this includes validating string lengths, nullable fields, data types, etc.

In many cases, validation of data requires cross-domain data.  As mentioned previously, each microservice should have enough data to complete its work.

## Each Microservice endpoint should require authentication
In addition, varying levels of authorization should be implemented to match those specified by the data owner.

## Service and API Versionining
Each microservice should be able to handle breaking changes to it's model and endpoints via API versioning.  The different versions and the discrepancies between the models should be communicated to developers via the OpenAPI spec documentation at a minimum.

The service should also give some indicator of which build number is currently running.  Microservice developers should be able to easily track down the source code for the running instance.