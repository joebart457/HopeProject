#pragma once


#include <memory>
#include <any>
#include <string>
#include <map>

#include "args.hpp"

#define TY_KEY int32_t

template <class Ty>
class Stor;

template <class Ty>
class Stack;

template <class Ty>
struct memory_loc {
	Ty addr;
};

template <class Ty>
class scope;

class native_fn;

class vm
{
public:
	vm(std::shared_ptr<std::map<int32_t, std::shared_ptr<native_fn>>> stdlib = nullptr);
	~vm();

	int run(char* buffer, std::size_t size);

private:

	template <class TyData>
	TyData read_as(std::istream& iss);
	std::string read_string(std::istream& iss);

	void check_magic_number(std::istream& handle);

	std::string read_symbol(std::istream& iss, uint32_t offset);
	std::any read_value_const(std::istream& iss, TY_KEY offset);

	template <class TyData>
	TyData pop_stack();

	// Instructions
	int fetch(std::istream& iss);
	int push_const(std::istream& iss);
	int pop(std::istream& iss);
	int mul(std::istream& iss);
	int div(std::istream& iss);
	int add(std::istream& iss);
	int sub(std::istream& iss);
	int gt(std::istream& iss);
	int gte(std::istream& iss);
	int lt(std::istream& iss);
	int lte(std::istream& iss);
	int eq(std::istream& iss);
	int neq(std::istream& iss);
	int jmp(std::istream& iss);
	int jz(std::istream& iss);
	int jnz(std::istream& iss);
	int call(std::istream& iss);
	int end(std::istream& iss);

	int execute_instruction(std::istream& iss);

	bool check_for_equality();
	template <class TyData>
	bool check_for_equality_helper();

	bool check_for_zero();
	template <class TyData>
	bool check_for_zero_helper();

	_args prepare_call(std::shared_ptr<native_fn> func);

	std::shared_ptr<Stor<TY_KEY>> _stor;
	std::shared_ptr<Stack<std::any>> _stack;
	std::shared_ptr<std::map<int32_t, std::shared_ptr<native_fn>>> _stdlib;
	uint32_t _symbolTableAddr, _symbolTableSize, _valueTableAddr, _valueTableSize, _instructionStart;
	bool _exitRequested{ false };
};

#include "Stor.hpp"
#include "Stack.hpp"
#include "native_fn.hpp"
#include "scope.hpp"
#include "definitions.hpp"

#include <iostream>
#include <sstream>
#include <cstdint>
#include <bit>


inline vm::vm(std::shared_ptr<std::map<int32_t, std::shared_ptr<native_fn>>> stdlib)
{
	_stack = std::make_shared<Stack<std::any>>();
	_stor = std::make_shared<Stor<TY_KEY>>();
	_stdlib = stdlib;
}

inline vm::~vm()
{
}

inline int vm::run(char* buffer, std::size_t size)
{
	_exitRequested = false;
	if (size < 42) return -8;
	_stack->init();
	_stor->init();
	std::string src = std::string(buffer, size);
	std::istringstream iss(src);
	std::istream& handle(iss);

	check_magic_number(handle);
	_symbolTableAddr = read_as<uint32_t>(handle);
	_symbolTableSize = read_as<uint32_t>(handle);
	_valueTableAddr = read_as<uint32_t>(handle);
	_valueTableSize = read_as<uint32_t>(handle);
	check_magic_number(handle);
	iss.seekg(_valueTableAddr + _valueTableSize, std::ios::beg);
	_instructionStart = _valueTableAddr + _valueTableSize;
	if (!iss.good()) return -1;
	int rc = 0;
	while (rc == 0 && iss && !_exitRequested) {
		rc = execute_instruction(iss);
	}
	return rc;
}

inline int vm::execute_instruction(std::istream& iss)
{
	int instruction = read_as<int32_t>(iss);
	if (instruction == INS_JMP) {
		return jmp(iss);
	}
	if (instruction == INS_ADD) {
		return add(iss);
	}
	if (instruction == INS_SUB) {
		return sub(iss);
	}
	if (instruction == INS_MUL) {
		return mul(iss);
	}
	if (instruction == INS_DIV) {
		return div(iss);
	}
	if (instruction == INS_GTE) {
		return gte(iss);
	}
	if (instruction == INS_GT) {
		return gt(iss);
	}
	if (instruction == INS_LTE) {
		return lte(iss);
	}
	if (instruction == INS_LT) {
		return lt(iss);
	}
	if (instruction == INS_JNZ) {
		return jnz(iss);
	}
	if (instruction == INS_JZ) {
		return jz(iss);
	}
	if (instruction == INS_CALL) {
		return call(iss);
	}
	if (instruction == INS_FETCH) {
		return fetch(iss);
	}
	if (instruction == INS_POP) {
		return pop(iss);
	}
	if (instruction == INS_CONST) {
		return push_const(iss);
	}
	if (instruction == INS_NEQ) {
		return neq(iss);
	}
	if (instruction == INS_EQ) {
		return eq(iss);
	}
	if (instruction == INS_END) {
		return end(iss);
	}

	return -1;
}

void inline vm::check_magic_number(std::istream& handle) {

	uint32_t magicNumber = read_as<uint32_t>(handle);
	if (magicNumber == 39) return;
	throw std::exception("invalid binary format");
}


inline std::string vm::read_symbol(std::istream& iss, uint32_t offset)
{
	if (offset > _symbolTableSize) throw std::exception("symbol access requested outside of allowed bounds");
	auto pos = iss.tellg();
	iss.seekg(_symbolTableAddr + offset);
	std::string symbol = read_string(iss);
	iss.seekg(pos);
	return symbol;
}


inline std::any vm::read_value_const(std::istream& iss, TY_KEY offset)
{
	if (offset > _valueTableSize) throw std::exception("value access requested outside of allowed bounds");
	auto pos = iss.tellg();
	iss.seekg(_valueTableAddr + offset, std::ios::beg);
	char type = read_as<char>(iss);
	std::any val;

	if (type == TY_INT) {
		val = std::make_any<int32_t>(read_as<int32_t>(iss));
	}
	else if (type == TY_STRING) {
		val = std::make_any<std::string>(read_string(iss));
	}
	else if (type == TY_DOUBLE) {
		val = std::make_any<double>(read_as<double>(iss));
	}
	else {
		throw std::exception("unsupported type encountered");
	}
	iss.seekg(pos); // Reset position
	return val;
}


template <class TyData>
inline TyData vm::read_as(std::istream& iss)
{
	if (!iss.good())throw std::exception("byte stream is invalid");
	TyData data;
	iss.read(reinterpret_cast<char*>(&data), sizeof(TyData));
	auto bytesRead = iss.gcount();
	if (bytesRead != sizeof(TyData)) throw std::exception("unable to read memory");
	return data;
}

template<class TyData>
inline TyData vm::pop_stack()
{
	std::any val = _stack->pop();
	if (val.type() != typeid(TyData)) throw std::exception("invalid stack data");
	return std::any_cast<TyData>(val);
}




inline std::string vm::read_string(std::istream& iss)
{
	uint32_t length = read_as<uint32_t>(iss);
	if (length == 0) return std::string("");
	if (!iss.good())throw std::exception("byte stream is invalid");
	std::string data(length, '\0');
	iss.read(&data[0], length);
	auto bytesRead = iss.gcount();
	if (bytesRead != length) throw std::exception("unable to read memory");
	return data;
}


// Instructions


inline int vm::fetch(std::istream& iss)
{
	TY_KEY location = read_as<TY_KEY>(iss);
	std::any val = _stor->get(location);
	_stack->push(val);
	return 0;
}


inline int vm::push_const(std::istream& iss)
{
	TY_KEY location = read_as<TY_KEY>(iss);
	std::any val = read_value_const(iss, location);
	_stack->push(val);
	return 0;
}

inline int vm::pop(std::istream& iss)
{
	TY_KEY location = read_as<TY_KEY>(iss);
	std::any val = _stack->pop();
	_stor->put(location, val);
	return 0;
}


inline int vm::mul(std::istream& iss)
{
	auto lhs = pop_stack<int32_t>();
	auto rhs = pop_stack<int32_t>();
	std::any ret = std::make_any<int32_t>(lhs * rhs);
	_stack->push(ret);
	return 0;
}


inline int vm::div(std::istream& iss)
{
	auto lhs = pop_stack<int32_t>();
	auto rhs = pop_stack<int32_t>();
	if (rhs == 0) throw std::exception("unable to divide by zero");
	std::any ret = std::make_any<int32_t>(lhs / rhs);
	_stack->push(ret);
	return 0;
}


inline int vm::add(std::istream& iss)
{
	auto lhs = pop_stack<int32_t>();
	auto rhs = pop_stack<int32_t>();
	std::any ret = std::make_any<int32_t>(lhs + rhs);
	_stack->push(ret);
	return 0;
}


inline int vm::sub(std::istream& iss)
{
	auto lhs = pop_stack<int32_t>();
	auto rhs = pop_stack<int32_t>();
	std::any ret = std::make_any<int32_t>(lhs - rhs);
	_stack->push(ret);
	return 0;
}


inline int vm::gt(std::istream& iss)
{
	std::any ret;
	auto lhs = _stack->pop();
	if (lhs.type() == typeid(int32_t)) {
		auto lhsVal = std::any_cast<int32_t>(lhs);
		auto rhs = pop_stack<int32_t>();
		ret = std::make_any<bool>(lhsVal > rhs);
	}
	else if (lhs.type() == typeid(double)) {
		auto lhsVal = std::any_cast<double>(lhs);
		auto rhs = pop_stack<double>();
		ret = std::make_any<bool>(lhsVal > rhs);
	}
	else {
		std::ostringstream oss;
		oss << "invalid comparison type: " << lhs.type().name();
		throw std::exception(oss.str().c_str());
	}
	_stack->push(ret);
	return 0;
}


inline int vm::gte(std::istream& iss)
{
	std::any ret;
	auto lhs = _stack->pop();
	if (lhs.type() == typeid(int32_t)) {
		auto lhsVal = std::any_cast<int32_t>(lhs);
		auto rhs = pop_stack<int32_t>();
		ret = std::make_any<bool>(lhsVal >= rhs);
	}
	else if (lhs.type() == typeid(double)) {
		auto lhsVal = std::any_cast<double>(lhs);
		auto rhs = pop_stack<double>();
		ret = std::make_any<bool>(lhsVal >= rhs);
	}
	else {
		std::ostringstream oss;
		oss << "invalid comparison type: " << lhs.type().name();
		throw std::exception(oss.str().c_str());
	}
	_stack->push(ret);
	return 0;
}


inline int vm::lt(std::istream& iss)
{
	std::any ret;
	auto lhs = _stack->pop();
	if (lhs.type() == typeid(int32_t)) {
		auto lhsVal = std::any_cast<int32_t>(lhs);
		auto rhs = pop_stack<int32_t>();
		ret = std::make_any<bool>(lhsVal < rhs);
	}
	else if (lhs.type() == typeid(double)) {
		auto lhsVal = std::any_cast<double>(lhs);
		auto rhs = pop_stack<double>();
		ret = std::make_any<bool>(lhsVal < rhs);
	}
	else {
		std::ostringstream oss;
		oss << "invalid comparison type: " << lhs.type().name();
		throw std::exception(oss.str().c_str());
	}
	_stack->push(ret);
	return 0;
}


inline int vm::lte(std::istream& iss)
{
	std::any ret;
	auto lhs = _stack->pop();
	if (lhs.type() == typeid(int32_t)) {
		auto lhsVal = std::any_cast<int32_t>(lhs);
		auto rhs = pop_stack<int32_t>();
		ret = std::make_any<bool>(lhsVal <= rhs);
	}
	else if (lhs.type() == typeid(double)) {
		auto lhsVal = std::any_cast<double>(lhs);
		auto rhs = pop_stack<double>();
		ret = std::make_any<bool>(lhsVal <= rhs);
	}
	else {
		std::ostringstream oss;
		oss << "invalid comparison type: " << lhs.type().name();
		throw std::exception(oss.str().c_str());
	}
	_stack->push(ret);
	return 0;
}


inline int vm::eq(std::istream& iss)
{
	std::any val = check_for_equality();
	_stack->push(val);
	return 0;
}


inline int vm::neq(std::istream& iss)
{
	std::any val = !check_for_equality();
	_stack->push(val);
	return 0;
}


inline int vm::jmp(std::istream& iss)
{
	auto location = read_as<int64_t>(iss);
	auto pos = iss.tellg();
	iss.seekg(_instructionStart + location, std::ios::beg);
	if (!iss.good()) {
		iss.seekg(pos, std::ios::beg);
		throw std::exception("attempted to seek past end of stream");
	}
	return 0;
}

inline int vm::jz(std::istream& iss) 
{
	if (check_for_zero()) {
		return jmp(iss);
	}
	read_as<int64_t>(iss); // throw away location
	return 0;
}


inline int vm::jnz(std::istream& iss) 
{
	if (!check_for_zero()) {
		return jmp(iss);
	}
	read_as<int64_t>(iss); // throw away location
	return 0;
}


inline int vm::call(std::istream& iss)
{
	if (_stdlib == nullptr) throw std::exception("calls to the standard library are disabled");
	auto fnIdentifier = read_as<int32_t>(iss);
	std::shared_ptr<native_fn> func;
	if (!_stdlib->count(fnIdentifier)){
		std::ostringstream oss;
		oss << "unable to find function identifier " << fnIdentifier << "in standard library";
		throw std::exception(oss.str().c_str());
	}
	func = (*_stdlib)[fnIdentifier];
	auto args = prepare_call(func);
	func->call(args);
	return 0;
}

inline int vm::end(std::istream& iss)
{
	int rc = pop_stack<int32_t>();
	_exitRequested = true;
	return rc;
}


// Helpers

template<class TyData>
inline bool vm::check_for_equality_helper()
{
	auto rhs = pop_stack<TyData>();
	auto lhs = pop_stack<TyData>();
	return lhs == rhs;
}

inline bool vm::check_for_equality() {
	auto rhs = _stack->peek();
	if (rhs.type() == typeid(char)) {
		return check_for_equality_helper<char>();
	}
	if (rhs.type() == typeid(int32_t)) {
		return check_for_equality_helper<int32_t>();
	}
	if (rhs.type() == typeid(double)) {
		return check_for_equality_helper<double>();
	}
	if (rhs.type() == typeid(std::string)) {
		return check_for_equality_helper<std::string>();
	}
	std::ostringstream oss;
	oss << "unable to compare non-primitive type: " << rhs.type().name();
	throw std::exception(oss.str().c_str());	
}


inline bool vm::check_for_zero()
{
	auto test = _stack->peek();
	if (test.type() == typeid(char)) {
		return check_for_zero_helper<char>();
	}
	if (test.type() == typeid(bool)) {
		return check_for_zero_helper<bool>();
	}
	if (test.type() == typeid(int32_t)) {
		return check_for_zero_helper<int32_t>();
	}
	if (test.type() == typeid(double)) {
		return check_for_zero_helper<double>();
	}
	std::ostringstream oss;
	oss << "unable to test non-primitive type: " << test.type().name();
	throw std::exception(oss.str().c_str());
}

template <class TyData>
inline bool vm::check_for_zero_helper()
{
	static_assert(std::is_arithmetic<TyData>::value, "Not an arithmetic type");
	auto testVal = pop_stack<TyData>();
	return testVal == 0;
}


inline _args vm::prepare_call(std::shared_ptr<native_fn> func)
{
	if (func == nullptr) throw std::exception("error during call to stdlib");
	auto params = func->parameters();
	auto it = params.rbegin();
	std::vector<std::any> stackArgs;
	for (it; it != params.rend(); ++it) {
		auto val = _stack->pop();
		if (it->szNativeType != val.type().name()) {
			std::ostringstream oss;
			oss << "type mismatch in call " << func->name() << ": expected type " << it->szNativeType << " but got " << val.type().name();
			throw std::exception(oss.str().c_str());
		}
		stackArgs.push_back(val);
	}
	return _args(stackArgs); // test
}

// End Helpers