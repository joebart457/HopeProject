#pragma once

#ifndef Std_Lib_h
#define Std_Lib_h



#include <any>
#include "args.hpp"


std::any Console_Log(_args args);
std::any Console_Log_Int(_args args);
std::any FileSystem_Exists(_args args);

#endif // !Std_Lib_h