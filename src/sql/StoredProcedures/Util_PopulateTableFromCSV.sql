if exists (select * from sysobjects where id = object_id(N'[dbo].[Util_PopulateTableFromCSV]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Util_PopulateTableFromCSV]
GO

CREATE PROCEDURE dbo.Util_PopulateTableFromCSV 

	@CSVList			NVARCHAR(MAX)
AS

/* 	
	Procedure:		Util_PopulateTableFromCSV	
	
	Description: 	Insert the separate values from a CSV list into the temporary table #tmpList

	Arguments:		@sList	 list of all items  as a CSV list

	Temp Table:		Predefined (need to be created before sp is called) 

					#tmpList
					- ListItem (NVarchar 100)

	History:		SH 22/11/2001 - Original script 

*/

DECLARE @TempString NVARCHAR (500)
DECLARE @PatternStart INT
DECLARE @Find INT
DECLARE @Entry NVARCHAR(100)

	SET @TempString=@CSVList
	

	WHILE 1=1
		BEGIN
			SET @Find=CHARINDEX (',', @TempString , 1 ) 
			--PRINT @Find
			IF @Find=0
				BEGIN
					IF LEN(@TempString)=0
						BEGIN
							BREAK
						END
					ELSE
						BEGIN
							SET @Find=LEN(@TempString)+1
						END
				END

			SET @Entry=SUBSTRING(@TempString, 1, @Find-1)
			INSERT INTO #tmpList (ListItem)
			VALUES(@Entry)

			SET @TempString=LTRIM(SUBSTRING(@TempString, @Find+1, LEN(@TempString)))

		END 