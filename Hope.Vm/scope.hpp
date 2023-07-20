#pragma once


#ifndef __INCLUDE_SCOPE_H
#define __INCLUDE_SCOPE_H

#include <string>
#include <map>
#include <memory>
#include <sstream>
#include <any>
#include <iostream>
#include <mutex>



template <class _Ty>
class scope {
	friend class Pickler;
public:
	scope() : m_id{ 0 } {}
	scope(unsigned int id) :m_id{ id } {}
	scope(const std::string& szAlias) : m_id{ 0 }, m_szAlias{ szAlias } {}

	scope(std::map<std::string, _Ty> lookup, const std::string& szAlias)
		:m_id{ 0 }, m_szAlias{ szAlias }
	{
		m_lookup = lookup;
	}

	~scope()
	{
		m_lookup.clear();
	}

	bool exists_local(const std::string& szKey)
	{
		std::scoped_lock(m_mtx);

		return m_lookup.count(szKey);
	}

	bool define(const std::string& szKey, _Ty val, bool overwrite = false)
	{
		std::scoped_lock(m_mtx);

		if (overwrite) {
			m_lookup[szKey] = val;
			return true;
		}
		else {
			if (!exists_local(szKey)) {
				m_lookup[szKey] = val;
				return true;
			}
			return false;
		}
	}

	bool remove(const std::string& szKey)
	{
		std::scoped_lock(m_mtx);

		if (!exists_local(szKey)) {
			return false;
		}
		else {
			m_lookup.erase(szKey);
			return true;
		}
	}

	bool assign(const std::string& szKey, _Ty val)
	{
		std::scoped_lock(m_mtx);

		if (exists_local(szKey)) {
			m_lookup[szKey] = val;
			return true;
		}
		return false;
	}

	bool get(const std::string& szKey, _Ty& out)
	{
		std::scoped_lock(m_mtx);

		if (exists_local(szKey)) {
			out = m_lookup[szKey];
			return true;
		}
		return false;
	}


	unsigned int id() const
	{
		std::scoped_lock(m_mtx);

		return m_id;
	}

	void set_id(unsigned int id)
	{
		std::scoped_lock(m_mtx);

		m_id = id;
	}

	

	std::shared_ptr<scope<_Ty>> copy()
	{
		std::scoped_lock(m_mtx);

		std::shared_ptr<scope<_Ty>> s = std::make_shared<scope<_Ty>>();
		for (auto it = m_lookup.begin(); it != m_lookup.end(); it++) {
			s->define(it->first, it->second, true);
		}
		return s;
	}


	std::map<std::string, _Ty>* lookup()
	{
		std::scoped_lock(m_mtx);

		return &m_lookup;
	}

private:
	std::string m_szAlias{ "" };
	unsigned int m_id{ 0 };
	std::map<std::string, _Ty> m_lookup;
	std::mutex m_mtx;
};

#endif