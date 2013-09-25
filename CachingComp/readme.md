Benchmark comparison of four different approaches to caching:
1. Redis with Twemproxy
2. Sharded Redis
3. Memcached with Twemproxy
4. Sharded Memcached

Typical result:
10000 iterations of each:
redis_proxy writes: 42.94 sec, reads: 29.88 sec, hit ratio: 100.00%
memcached_proxy writes: 30.98 sec, reads: 30.69 sec, hit ratio: 100.00%
sharded_redis writes: 33.50 sec, reads: 32.96 sec, hit ratio: 100.00%
sharded_memcached writes: 33.12 sec, reads: 32.20 sec, hit ratio: 100.00%