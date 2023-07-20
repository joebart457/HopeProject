#pragma once
#include <string>
#include <vector>
#include "args.hpp"
#include "param.hpp"

typedef std::any(*func)(_args);

class native_fn: public std::enable_shared_from_this<native_fn> {
public:
	native_fn(const std::string& szName, func fn)
		:m_szName{ szName }, m_hFn{ fn } {}
	~native_fn() {  }

	std::any call(_args args);

	std::shared_ptr<native_fn> registerParameter(const param& p);

	template <class Ty>
	std::shared_ptr<native_fn> registerParameter(const std::string& szName);

	std::string name() { return m_szName; }
	std::vector<param> parameters() { return m_parameters; }

private:
	std::string m_szName{ "" };
	func m_hFn;
	std::vector<param> m_parameters;
};


inline std::shared_ptr<native_fn> native_fn::registerParameter(const param& p)
{
	m_parameters.push_back(p);
	return std::static_pointer_cast<native_fn>(shared_from_this());
}

template<class Ty>
inline std::shared_ptr<native_fn> native_fn::registerParameter(const std::string& szName)
{
	param p;
	p.szName = szName;
	p.szNativeType = typeid(Ty).name();
	m_parameters.push_back(p);
	return std::static_pointer_cast<native_fn>(shared_from_this());
}


inline std::any native_fn::call(_args args)
{
	return m_hFn(args);
}

