C:

CD c:\Kafka\bin\windows\

CALL kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic users

CALL kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic bookings

CALL kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic payments

CALL kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic notifications