SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 12/02/2026
-- Description:	create a new room with given name
-- =============================================
CREATE PROCEDURE dbo.createRoom
	-- Add the parameters for the stored procedure here
	@name NVARCHAR(20) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.rooms (name)
		OUTPUT INSERTED.id
		VALUES(@name);
END
GO
