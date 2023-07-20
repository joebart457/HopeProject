#include "stdlib.hpp"


std::any Console_Log(_args args) {
	auto msg = args.get<std::string>(0);
	std::cout << msg << std::endl;
	return 0;
}

std::any Console_Log_Int(_args args) {
	auto msg = args.get<int32_t>(0);
	std::cout << msg << std::endl;
	return 0;
}