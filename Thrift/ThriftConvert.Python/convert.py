import  sys
sys.path.append("gen-py")
from thrift.transport import TSocket 
from thrift.protocol import TBinaryProtocol

from convertLength import *
print("[Client] started")
socket = TSocket.TSocket("localhost", 9090) 
socket.open()
print("[Client] opened socket")
protocol = TBinaryProtocol.TBinaryProtocol(socket) 

client = ConvertSvc.Client(protocol)
units = client.availableUnits_func()
print("Available units: %s" % ', '.join(units))
input = raw_input('Please enter length to convert: ')
msg = client.convert_func(input);
print("[Client] received: %s" % msg)