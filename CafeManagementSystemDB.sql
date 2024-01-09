USE [master]
GO
/****** Object:  Database [CafeManagementSystemDB]    Script Date: 10/01/2024 00:09:24 ******/
CREATE DATABASE [CafeManagementSystemDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CafeManagementSystemDB', FILENAME = N'C:\Users\kargi\CafeManagementSystemDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CafeManagementSystemDB_log', FILENAME = N'C:\Users\kargi\CafeManagementSystemDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [CafeManagementSystemDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CafeManagementSystemDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CafeManagementSystemDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CafeManagementSystemDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CafeManagementSystemDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CafeManagementSystemDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CafeManagementSystemDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CafeManagementSystemDB] SET  MULTI_USER 
GO
ALTER DATABASE [CafeManagementSystemDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CafeManagementSystemDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CafeManagementSystemDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CafeManagementSystemDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CafeManagementSystemDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CafeManagementSystemDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [CafeManagementSystemDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [CafeManagementSystemDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [CafeManagementSystemDB]
GO
/****** Object:  Table [dbo].[Bill]    Script Date: 10/01/2024 00:09:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bill](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [varchar](200) NULL,
	[name] [varchar](255) NULL,
	[email] [varchar](255) NULL,
	[contactNumber] [varchar](20) NULL,
	[paymentMethod] [varchar](50) NULL,
	[totalAmount] [int] NULL,
	[createdBy] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[uuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BillProduct]    Script Date: 10/01/2024 00:09:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BillProduct](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[productId] [int] NULL,
	[billId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 10/01/2024 00:09:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 10/01/2024 00:09:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NULL,
	[categoryId] [int] NULL,
	[description] [varchar](255) NULL,
	[price] [int] NULL,
	[status] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10/01/2024 00:09:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](250) NULL,
	[contactNumber] [varchar](20) NULL,
	[email] [varchar](50) NULL,
	[password] [varchar](250) NULL,
	[status] [varchar](20) NULL,
	[role] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_email] UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BillProduct]  WITH CHECK ADD FOREIGN KEY([billId])
REFERENCES [dbo].[Bill] ([id])
GO
ALTER TABLE [dbo].[BillProduct]  WITH CHECK ADD FOREIGN KEY([productId])
REFERENCES [dbo].[Product] ([id])
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([categoryId])
REFERENCES [dbo].[Category] ([id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category]
GO
USE [master]
GO
ALTER DATABASE [CafeManagementSystemDB] SET  READ_WRITE 
GO
