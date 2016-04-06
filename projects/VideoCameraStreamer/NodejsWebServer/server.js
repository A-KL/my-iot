var http = require('http');
var addon = require("./MemoryStatusAddon");

http.createServer(function (req, res) 
{
	//res.writeHead(200, { 'Content-Type': 'text/plain' });
	//res.end('Hello World\n');

	var memObj = addon.GlobalMemoryStatusEx();
	res.writeHead(200, { 'Content-Type': 'text/plain' });
	res.write('*************************************************\n');
	res.write('Percent of memory in use: ' + memObj.load + '\n');
	res.write('KB of physical memory: ' + memObj.physKb + '\n');
	res.write('KB of free physical memory: ' + memObj.freePhysKb + '\n');
	res.write('KB of paging file: ' + memObj.pageKb + '\n');
	res.write('KB of free paging file: ' + memObj.freePageKb + '\n');
	res.write('KB of virtual memory: ' + memObj.virtualKb + '\n');
	res.write('KB of free virtual memory: ' + memObj.freeVirtualKb + '\n');
	res.write('KB of free extended memory: ' + memObj.freeExtKb + '\n');
	res.end('*************************************************\n');

}).listen(1337);