IF EXISTS ( SELECT 1 FROM dbo.SysObjects WHERE id=OBJECT_ID('dbo.SystemUser_Create') AND OBJECTPROPERTY(id,'IsProcedure')=1)
	BEGIN
		DROP PROCEDURE dbo.SystemUser_Create
	END
GO

CREATE PROCEDURE dbo.SystemUser_Create	
	@UserId					UNIQUEIDENTIFIER,
	@FirstName				NVARCHAR(100),
	@Surname				NVARCHAR(100),
	@HasNewsLetter			BIT,
	@PrimaryEmailAddressId	INT,
	@PhoneNumberId			INT,						
	@SystemUserId			INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
	-- check for duplicate user
	IF EXISTS (SELECT 1
			   FROM dbo.SystemUser lu
			   INNER JOIN dbo.SystemUser_EmailAddress lem
					   ON lem.SystemUserId=lu.SystemUserId
			   WHERE LOWER(FirstName)=LOWER(@FirstName) AND
					 LOWER(Surname)=LOWER(@Surname) AND
					 lem.EmailAddressId=@PrimaryEmailAddressId)
		BEGIN
				-- return -99 to indicate a duplicate user created
				SET @SystemUserId=-99
		END
	
	ELSE
	
		BEGIN TRAN
		
		BEGIN						
				INSERT INTO dbo.SystemUser
				(
					UserId,
					FirstName,
					Surname,
					HasNewsLetter,
					PhoneNumberId
				)
				VALUES
				(
					@UserId,
					@FirstName,
					@Surname,
					@HasNewsLetter,
					@PhoneNumberId
				)
				
				SET @SystemUserId = SCOPE_IDENTITY()				
		END		
		
		IF @@ERROR <> 0
			BEGIN
				GOTO ErrorHandler
			END
			
		INSERT INTO dbo.SystemUser_EmailAddress  (SystemUserId, EmailAddressId, EmailAddressOrder)
		VALUES (@SystemUserId, @PrimaryEmailAddressId, 1)
		
		IF @@ERROR <> 0
			BEGIN
				GOTO ErrorHandler
			END
		
Finally:
			IF @@TRANCOUNT > 0
				COMMIT
					

			RETURN
	
ErrorHandler:

			IF @@TRANCOUNT > 0
				ROLLBACK
		
			GOTO Finally
				
END
GO