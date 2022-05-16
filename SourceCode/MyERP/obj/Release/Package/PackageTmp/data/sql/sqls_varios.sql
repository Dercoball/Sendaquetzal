
--	REVISAR HOROMETROS DE UN EQUIPO
select r.id_equipo, e.numero_economico, r.orometro, r.id_requisicion, r.fecha_creacion,
e.orometro_ultimo_mantenimiento, r.descripcion
from requisicion r 
JOIN equipo e ON (e.id_equipo = r.id_equipo)
where e.numero_economico = 'CF07'
ORDER BY r.id_requisicion ASC;

-- ASIGNAR UN HORÓMETRO ( campo orometro) a UN EQUIPO, es por id_requisicion esta requisicion asu vez esta asociada al equipo  Por tanto se asigna orometro
-- a la requisicion
update requisicion set orometro = 4226.3 where id_requisicion  = 2880;


-- ACTUALIZAR VALORES DE SOLICITUD COMBUSTIBLE, DE ACUERDO AL  HEADER DE LA SOLICITUD
UPDATE detalle_solicitud_combustible SET
 id_tipo_combustible = 2,
 id_obra = 819
 WHERE id_solicitud_combustible in ( 
select id_solicitud_combustible from solicitud_combustible 
WHERE id_usuario_solicita= '1055'
 );



