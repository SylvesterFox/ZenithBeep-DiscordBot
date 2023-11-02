echo "drop table Script"
psql -d grechkadb -U postgres <<EOF 
DROP TABLE IF EXISTS roles CASCADE;
DROP TABLE IF EXISTS guilds CASCADE;
DROP TABLE IF EXISTS roomers_lobbys CASCADE;
DROP TABLE IF EXISTS rooms CASCADE;
EOF
