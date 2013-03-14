#!/usr/bin/python

"""
Protobuf via 0MQ POC
"""

import sys
import user_pb2
import zmq

host = "tcp://aarkhipov00:5555"

def main():
	context = zmq.Context()
	socket = context.socket(zmq.REQ)
	socket.connect(host)

	while True:

		## Get user id from user
		try:
			uid = int(raw_input("\nUser ID: "))
		except ValueError:
			print("Oops! ID must be an int.")
			continue

		## Create request, serialize and send
		ureq = user_pb2.UserRequest()
		ureq.UserId = uid
		socket.send (ureq.SerializeToString())

		## Receive proto user
		response = socket.recv()
		if(len(response) == 0):
			print("User not found.")
			continue

		## Deserialize user
		user = user_pb2.User()
		user.ParseFromString(response)

		## Print results
		print('ID: {0}, Name: {1}, Active: {2}'.format(user.Id, user.Name ,user.Active))
		print('Meta:'),
		printMeta(user.Metadata)

def printMeta(kv):
	if(len(kv) == 0):
		return

	for x in kv:
		print('\t{0}: {1}'.format(x.Key, x.Value))

if __name__ == '__main__':
    main()
