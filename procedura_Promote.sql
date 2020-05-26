if OBJECT_ID('dbo.Promote', 'P') IS NOT NULL DROP PROCEDURE dbo.Promote;
go
create procedure dbo.Promote(@studies nvarchar(100), @semester int)
as
begin
	set nocount on
	declare @IdEnrollmentNext int, @IdEnrollmentCurrent int, @idStudies int;

	select @IdEnrollmentCurrent = e.IdEnrollment, @idStudies = e.IdStudy
	from dbo.Enrollment e inner join dbo.Studies s on e.IdStudy = s.IdStudy 
	where s.Name = @studies and e.Semester = @semester;

	set @IdEnrollmentNext = (select e.IdEnrollment
	from dbo.Enrollment e inner join dbo.Studies s on e.IdStudy = s.IdStudy 
	where s.Name = @studies and e.Semester = @semester + 1);

	if (@IdEnrollmentNext is null)
	begin
		set @IdEnrollmentNext = (select isnull(max(IDEnrollment), 0) + 1 from dbo.Enrollment);
		insert into dbo.Enrollment(IdEnrollment, Semester, IdStudy, StartDate)
		values(@IdEnrollmentNext, @semester + 1, @idStudies, cast(getdate() as date));
	end

	update dbo.Student
	set IdEnrollment = @IdEnrollmentNext
	where IdEnrollment = @IdEnrollmentCurrent;
end
go

exec dbo.Promote 'Informatyka studia dzienne', 2;