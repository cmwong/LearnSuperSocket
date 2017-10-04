### BasicClientServer02
* change transmit string to json (Command RequestAdd, ResponseAdd)
* sync RequestAdd method
* use enum as our command
* hide the EasyClient Send method (sending raw command)
* only send by our Request method
* data from server Expose to event (so whoever is using our client, just hook to the event they r interested.)
