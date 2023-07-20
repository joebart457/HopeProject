#pragma once

#include <vector>
#include <any>
#include <sstream>

class _args
{
public:
	_args(std::vector<std::any>& data)
	{
		m_data = data;
	}
	_args() {}
	~_args() {}

	std::any& at(unsigned int index)
	{
		return m_data.at(index);
	}

	size_t size()
	{
		return m_data.size();
	}

	template <typename _Ty>
	_Ty get(unsigned int index)
	{
		if (index < m_data.size()) {
			if (m_data.at(index).type() != typeid(_Ty)) {
				std::ostringstream oss;
				oss << "type mismatch, expected type " << typeid(_Ty).name() << " but got " << m_data.at(index).type().name();
				throw std::exception(oss.str().c_str());
			}
			return std::any_cast<_Ty>(m_data.at(index));
		}
		std::ostringstream oss;
		oss << "index out of range " << m_data.size() << " < " << index;
		throw std::exception(oss.str().c_str());
	}

	std::vector<std::any> data()
	{
		return m_data;
	}

private:
	std::vector<std::any> m_data;
};