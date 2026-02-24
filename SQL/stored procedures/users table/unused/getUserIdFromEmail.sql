SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 12/02/2026
-- Description: Get id of user with given email
-- =============================================
CREATE PROCEDURE dbo.getUserIdFromEmail
	-- Add the parameters for the stored procedure here
	@email NVARCHAR(320) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT id
		FROM dbo.users
		WHERE email = @email;
END
GO
