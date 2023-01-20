--Modificaciones a la table inversionistas 
ALTER TABLE inversionista add fecha_registro datetime not null default(GETDATE())
GO
ALTER TABLE inversionista add fecha_edicion datetime 
GO
ALTER TABLE inversionista add status bit not null default(1)
GO
EXEC sp_RENAME 'inversionista.porcentaje_interes_anual', 'porcentaje_utilidad_sugerida', 'COLUMN'
GO

--Tabla de status de inversiones
CREATE TABLE status_inversion
(
	id_status_inversion	int not null ,
	nombre nvarchar(150) not null ,
	color nvarchar(150) not null ,

	constraint PK_status_inversion primary key(id_status_inversion)
)

insert into status_inversion values(1, 'Vigente', 'badge badge-info')
insert into status_inversion values(2, 'Liquidado', 'badge badge-warning')

--Modificaciones a la tablde inversion
drop TABLE [inversion]
go
CREATE TABLE [dbo].[inversion](
	[id_inversion] [int] IDENTITY(1,1) NOT NULL,
	[id_inversionista] [int] not NULL,
	id_status_inversion int not null , 
	[fecha] [date] NULL,
	[monto] [float] NULL,
	porcentaje_utilidad float not null,
    utilidad_pesos float not null,
	plazo int not null, 
	inversion_utilidad  float,
	monto_retiro  float,
	fecha_retiro  datetime , 
	[comprobante_desposito] [varchar](max) NULL,
	[comprobante_retiro] [varchar](max) NULL,
	[eliminado] [int] NULL,

	CONSTRAINT PK_inversion primary KEY(id_inversion) , 
	CONSTRAINT FK_inversion_inversionista FOREIGN KEY(id_inversionista) REFERENCES inversionista(id_inversionista),
	CONSTRAINT FK_inversion_status_inversion FOREIGN KEY(id_status_inversion) REFERENCES status_inversion(id_status_inversion)
)

