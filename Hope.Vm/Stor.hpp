#pragma once

#include <vector>
#include <any>
#include <memory>

template <class Ty1>
class Slice;

template <class Ty>
class Stor
{
public:
	Stor();
	~Stor();

	std::any get(Ty key);
	void put(Ty key, std::any& val);

	void push_scope();
	void pop_scope();
	void init();

private:

	void check_scope();
	std::any get_helper(Ty key, size_t index);

	std::shared_ptr<Slice<Ty>> _currentScope;

	std::vector<std::shared_ptr<Slice<Ty>>> _scopes;
};


#include "Slice.hpp"

template<class Ty>
inline Stor<Ty>::Stor()
{
}

template<class Ty>
inline Stor<Ty>::~Stor()
{
}

template<class Ty>
inline std::any Stor<Ty>::get(Ty key)
{
	check_scope();
	if (_currentScope->exists(key)) return _currentScope->get(key);
	return get_helper(key, _scopes.size() - 2);
}

template<class Ty>
inline void Stor<Ty>::put(Ty key, std::any& val)
{
	check_scope();
}

template<class Ty>
inline void Stor<Ty>::push_scope()
{
	auto temp = std::make_shared<Slice<Ty>>();
	_currentScope = temp;
	_scopes.push_back(temp);
}

template<class Ty>
inline void Stor<Ty>::pop_scope()
{
	_scopes.pop_back();
	if (_scopes.empty()) _currentScope = nullptr;
	else _currentScope = _scopes.back();
}

template<class Ty>
void Stor<Ty>::init()
{
	_currentScope = nullptr;
	_scopes.clear();
	push_scope();
}

template<class Ty>
inline void Stor<Ty>::check_scope()
{
	if (_currentScope == nullptr) throw std::exception("current scope is nullptr");
}

template<class Ty>
inline std::any Stor<Ty>::get_helper(Ty key, size_t index)
{
	if (index >= _scopes.size()) throw std::exception("key not found in stack");
	if (_scopes.at(index)->exists(key)) return _scopes.at(index)->get(key);
	return get_helper(key, index - 1);
}
