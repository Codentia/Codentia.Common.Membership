if exists (select * from sysobjects where id = object_id(N'[dbo].[DataLoad_UserDetails]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DataLoad_UserDetails]
GO

CREATE PROCEDURE [dbo].[DataLoad_UserDetails]
	@FirstName				NVARCHAR(100),
	@Surname				NVARCHAR(100),
	@HasNewsLetter			BIT,
	@EmailAddressId			INT,
	@PhoneNumber			NVARCHAR(11),
	@RoleName				NVARCHAR(50),
	@Password				NVARCHAR(128)=NULL,
	@PasswordSalt			NVARCHAR(128)=NULL,	
	@SystemUserId			INT OUTPUT
AS					  

SET NOCOUNT ON

DECLARE @RoleId uniqueidentifier
DECLARE @UserId uniqueidentifier
DECLARE @AppId uniqueidentifier
DECLARE @EmailAddress NVARCHAR(256)

SELECT @EmailAddress=EmailAddress FROM dbo.EmailAddress WHERE EmailAddressId=@EmailAddressId


SELECT TOP 1 @AppId = ApplicationId FROM dbo.aspnet_Applications
SET @UserId=NEWID()

IF @Password IS NULL OR @PasswordSalt IS NULL
	BEGIN
		-- password and salt evaluate to 1234567*
		SET @Password='Pz4Ubap3/OTj/lOhlqybNS+Ft84='
		SET @PasswordSalt='XwzMaV0d1D9k3T1WhVCBEQ=='
	END
	
SELECT @RoleId=RoleId FROM aspnet_Roles WHERE RoleName=@RoleName

INSERT INTO [aspnet_Users] ([ApplicationId],[UserId],[UserName], [LoweredUserName], [IsAnonymous],[LastActivityDate])
VALUES (@AppId, @UserId, @EmailAddress, LOWER(@EmailAddress), 0, GETDATE())

SELECT @UserId=UserId FROM aspnet_Users WHERE UserName=@EmailAddress

INSERT INTO [aspnet_Membership] ([ApplicationId],[UserId],[Password], [PasswordFormat], [PasswordSalt],[Email],[LoweredEmail],[PasswordQuestion]
           			,[PasswordAnswer],[IsApproved],[IsLockedOut],[CreateDate],[LastLoginDate],[LastPasswordChangedDate]
				,[LastLockoutDate], [FailedPasswordAttemptCount],[FailedPasswordAttemptWindowStart]
           			,[FailedPasswordAnswerAttemptCount],[FailedPasswordAnswerAttemptWindowStart])

VALUES (@AppId, @UserId, @Password, 1, @PasswordSalt, @EmailAddress, LOWER(@EmailAddress), 'a', 'ILm8E2KqeWblos9QILNzLqHwrrc=',1, 0, GETDATE(), GETDATE(), GETDATE(), 
	'01/01/1754 00:00:00', 0, '01/01/1754 00:00:00', 0, '01/01/1754 00:00:00')
	

-- PhoneNumber
DECLARE @PhoneNumberId INT

IF @PhoneNumber IS NOT NULL
	BEGIN	
		EXEC dbo.PhoneNumber_Create	
					@PhoneNumber	= @PhoneNumber,
					@PhoneNumberId	= @PhoneNumberId OUTPUT
	END				
	
-- local user
EXEC dbo.SystemUser_Create 
			@UserId					= @UserId, 
			@FirstName				= @FirstName, 
			@Surname				= @Surname, 
			@HasNewsLetter			= @HasNewsLetter, 
			@PrimaryEmailAddressId	= @EmailAddressId, 
			@PhoneNumberId			= @PhoneNumberId, 
			@SystemUserId			= @SystemUserId OUTPUT

-- set role		
EXEC dbo.SystemUser_SetRole @SystemUserId=@SystemUserId, @RoleName=@RoleName
