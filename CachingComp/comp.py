#!/usr/bin/env python

"""
Performance tesing of four different caching solutions.
To run: ./comp.py [iterations]

https://github.com/andymccurdy/redis-py
https://github.com/linsomniac/python-memcached
http://amix.dk/blog/post/19367
"""

import sys
import memcache
import redis
from hash_ring import HashRing
import time

modes = ['redis_proxy', 'memcached_proxy', 'sharded_redis', 'sharded_memcached']

cluster = {
	'redis_proxy':'10.129.1.48:6379',
	'memcached_proxy':'10.129.1.48:11211',
	'sharded_redis':['sfredis00:6381','sfredis00:6382','sfredis00:6383','sfredis00:6384','sfredis01:6381','sfredis01:6382','sfredis01:6383','sfredis01:6384'],
	'sharded_memcached':['sfredis00:11211','sfredis00:11212','sfredis00:11213','sfredis00:11214','sfredis01:11211','sfredis01:11212','sfredis01:11213','sfredis01:11214']
}

ring = None
	
# Memcached servers hash ring

def GetClient(key, mode):
	if mode.startswith('sharded'):
		server = ring.get_node(key)
	else:
		server = cluster[mode]
	
	if mode.find('memcached') > -1:
		return memcache.Client([server], debug=0)
	else:
		parts = server.split(':')
		return redis.Redis(host = parts[0], port = int(parts[1]))

def CreateKey(x):
	return 'my_caching_key_{0}'.format(x)

def SetCache(mode, key, value):
	client = GetClient(key, mode)
	return client.set(key, value)

def GetCache(mode, key):
	client = GetClient(key, mode)
	return client.get(key)

def RemoveCache(mode, key):
	client = GetClient(key, mode)
	return client.delete(key)

def ConfigureHash(mode):
	global ring
	ring = HashRing(cluster[mode]

def main():
	hits = 0
	misses = 0
	results = []

	if len(sys.argv) > 1:
		count = int(sys.argv[1])
	else:
		count = 10

	for mode in modes:
		ConfigureHash(mode)
		writes = 0.0
		reads = 0.0

		print 'Writing using', mode
		wstart = time.time()
		for x in xrange(0,count):
			key = CreateKey(x)
			#print "{0} W {1}:{2}".format(mode, key, x)
			retval = SetCache(mode, key, 'my_caching_value_%s' % x)
			if retval == 0 or retval is None:
				raise Exception('Error adding value.')
		writes = time.time() - wstart

		print 'Reading using', mode
		rstart = time.time()
		for x in xrange(0,count):
			key = CreateKey(x)
			retval = GetCache(mode, key)
			#print "{0} R {1}:{2}".format(mode, key, retval)
			if retval != None:
				hits += 1
			else:
				misses += 1
		reads = time.time() - rstart

		print 'Cleaning up after', mode
		for x in xrange(0,count):
			key = CreateKey(x)
			retval = RemoveCache(mode, key)
			if retval != 1:
				raise Exception('Error deleting value.')

		ratio = hits / (hits + misses)
		results.append([mode, writes, reads, ratio])
	
	print "\n%s iterations of each:" % count
	for result in results:
		print "{0} writes: {1:.2f} sec, reads: {2:.2f} sec, hit ratio: {3:.2%}".format(result[0], result[1], result[2], result[3])

if __name__ == "__main__":
	main()