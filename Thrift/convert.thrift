service ConvertSvc{
	string convert_func(1: string input)
	list<string> availableUnits_func()
}