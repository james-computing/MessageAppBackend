SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 12/02/2026
-- Description:	update room name
-- =============================================
CREATE PROCEDURE dbo.updateRoomName
	-- Add the parameters for the stored procedure here
	@id INT = NULL,
	@name NVARCHAR(20) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.rooms
	SET name = @name
	WHERE id = @id;
END
GO
