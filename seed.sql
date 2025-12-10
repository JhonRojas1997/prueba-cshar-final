-- Seed initial data to satisfy Foreign Keys
INSERT INTO "Departamentos" ("Id", "Nombre") VALUES (1, 'IT') ON CONFLICT ("Id") DO NOTHING;
INSERT INTO "Cargos" ("Id", "Nombre") VALUES (1, 'Desarrollador') ON CONFLICT ("Id") DO NOTHING;
INSERT INTO "NivelesEducativos" ("Id", "Nombre") VALUES (1, 'Profesional') ON CONFLICT ("Id") DO NOTHING;
