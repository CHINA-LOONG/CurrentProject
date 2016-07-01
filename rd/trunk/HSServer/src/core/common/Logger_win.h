#pragma once
#ifdef _WIN32
#ifndef _LOG4CXX_WIN_
#define _LOG4CXX_WIN_
#include <string>
#include <iostream>
using namespace std;

namespace log4cxx
{
	class PropertyConfigurator
	{
	public:
		static void configureAndWatch(string){}
	};

	class Logger;

	typedef Logger* LoggerPtr;

	class Logger
	{
	public:
		Logger(){}

		static LoggerPtr getLogger(string)
		{
			if(logger_ == NULL)
			{
				logger_ = new Logger();
			}
			return logger_;
		}

		bool isDebugEnabled()
		{
#ifdef _DEBUG
			return true;
#else
			return false;
#endif
		}

		bool isTraceEnabled()
		{
#ifdef _DEBUG
			return true;
#else
			return false;
#endif
		}

		bool isInfoEnabled()
		{
#ifdef _DEBUG
			return true;
#else
			return false;
#endif
		}

		bool isWarnEnabled()
		{
#ifdef _DEBUG
			return false;
#else
			return false;
#endif
		}

		bool isErrorEnabled()
		{
#ifdef _DEBUG
			return true;
#else
			return true;
#endif
		}

		bool isFatalEnabled()
		{
#ifdef _DEBUG
			return true;
#else
			return true;
#endif
		}

		static Logger* logger_;
	};
};

#define LOG4CXX_DEBUG(logger, message) { \
	if (logger->isDebugEnabled()) { cout << message << endl; }}

#define LOG4CXX_TRACE(logger, message) { \
	if (logger->isTraceEnabled()) { cout << message << endl; }}

#define LOG4CXX_INFO(logger, message) { \
	if (logger->isInfoEnabled()) { cout << message << endl; }}

#define LOG4CXX_WARN(logger, message) { \
	if (logger->isWarnEnabled()) { cout << message << endl; }}

#define LOG4CXX_ERROR(logger, message) { \
	if (logger->isErrorEnabled()) { cout << message << endl; }}

#define LOG4CXX_FATAL(logger, message) { \
	if (logger->isFatalEnabled()) { cout << message << endl; }}

#endif
#endif
