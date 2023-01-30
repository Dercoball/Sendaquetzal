--modificacion a la tabla dias paro
ALTER TABLE dias_paro
ADD  id_plaza int NULL;

--modificacion a la tabla calendario
ALTER TABLE calendario
ADD es_laboral bit not null default(1)
;
