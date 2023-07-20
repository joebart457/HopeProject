#pragma once

#include <memory>
#include <map>
#include <cstdlib>
#include "scope.hpp"
#include "native_fn.hpp"
#include "stdlib.hpp"

class ContextBuilder 
{
public:
	static std::shared_ptr<std::map<int32_t, std::shared_ptr<native_fn>>> create_std_lib() 
	{
		auto stdlib = std::make_shared<std::map<int32_t, std::shared_ptr<native_fn>>>();

		(*stdlib)[1] = std::make_shared<native_fn>("Console.Log", Console_Log)
			->registerParameter<std::string>("msg");

		(*stdlib)[2] = std::make_shared<native_fn>("Console.LogInt", Console_Log_Int)
			->registerParameter<int32_t>("msg");

		return stdlib;
	}

};