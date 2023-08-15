USE [master]
GO

/****** Object:  Database [UserManagementDb]    Script Date: 8/15/2023 11:56:43 PM ******/
CREATE DATABASE [UserManagementDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'UserManagementDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\UserManagementDb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'UserManagementDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\UserManagementDb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [UserManagementDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [UserManagementDb] SET ANSI_NULL_DEFAULT OFF  
GO

ALTER DATABASE [UserManagementDb] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [UserManagementDb] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [UserManagementDb] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [UserManagementDb] SET ARITHABORT OFF 
GO

ALTER DATABASE [UserManagementDb] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [UserManagementDb] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [UserManagementDb] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [UserManagementDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [UserManagementDb] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [UserManagementDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [UserManagementDb] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [UserManagementDb] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [UserManagementDb] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [UserManagementDb] SET  ENABLE_BROKER 
GO

ALTER DATABASE [UserManagementDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [UserManagementDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [UserManagementDb] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [UserManagementDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [UserManagementDb] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [UserManagementDb] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [UserManagementDb] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [UserManagementDb] SET RECOVERY FULL 
GO

ALTER DATABASE [UserManagementDb] SET  MULTI_USER 
GO

ALTER DATABASE [UserManagementDb] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [UserManagementDb] SET DB_CHAINING OFF 
GO

ALTER DATABASE [UserManagementDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [UserManagementDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [UserManagementDb] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [UserManagementDb] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [UserManagementDb] SET QUERY_STORE = OFF
GO

ALTER DATABASE [UserManagementDb] SET  READ_WRITE 
GO


