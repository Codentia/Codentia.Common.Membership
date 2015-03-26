if exists (select * from sysobjects where id = object_id(N'dbo.DataLoad_Roles') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure dbo.DataLoad_Roles
GO

CREATE PROCEDURE dbo.DataLoad_Roles
	@ApplicationName		NVARCHAR(256),
	@RoleNames				NVARCHAR(300)	
AS	

SET NOCOUNT ON

DECLARE @AppId uniqueidentifier
DECLARE @CurrentRoleName NVARCHAR(100)
DECLARE @RoleUID uniqueidentifier

SELECT @AppId = ApplicationId FROM aspnet_Applications
WHERE ApplicationName=@ApplicationName

CREATE TABLE #tmpList (ListItem		NVARCHAR(100))

EXEC dbo.Util_PopulateTableFromCSV	@CSVList=@RoleNames

WHILE EXISTS (SELECT 1 FROM #tmpList)
	BEGIN
	
			SET @RoleUID=NEWID();
			SELECT TOP 1 @CurrentRoleName=ListItem FROM #tmpList
			
			IF NOT EXISTS (SELECT 1 FROM dbo.aspnet_Roles WHERE RoleName=@CurrentRoleName)
				BEGIN
				
					INSERT INTO aspnet_Roles (ApplicationId, RoleId, RoleName, LoweredRoleName)
					VALUES (@AppId, @RoleUID, @CurrentRoleName, LOWER(@CurrentRoleName))
					
				END
			
			DELETE #tmpList WHERE ListItem=@CurrentRoleName
	END
