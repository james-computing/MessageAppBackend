SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 19/01/2026
-- Description:	delete a user
-- =============================================
CREATE PROCEDURE dbo.deleteUser
	-- Add the parameters for the stored procedure here
	@id INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE
	FROM db.users
	WHERE id = @id;
END
GO
