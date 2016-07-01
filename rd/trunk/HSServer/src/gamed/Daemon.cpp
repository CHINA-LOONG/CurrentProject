
#include "Daemon.h"
#include <string.h>
#include "common/string-util.h"
#include "common/SysLog.h"
#include "EventQueue.h"
#include "GameNetHandler.h"
#include "GameEventHandler.h"
#include "GameDataHandler.h"
#include "gdhandler.h"
#include "logic/UserCtrl.h"
#include "ShareMemory/ShareMemHandler.h"
#include "logic/GameConfig.h"
#include "logic/GameConfigMgr.h"

#ifndef _WIN32
#include <execinfo.h>
#include <sys/sysinfo.h>
#include <linux/unistd.h>
#include <linux/kernel.h>
#include <fcntl.h>
#include <log4cxx/basicconfigurator.h>
#include <log4cxx/logger.h>
#include <log4cxx/propertyconfigurator.h>
#else
#include "common/Logger_win.h"

log4cxx::LoggerPtr log4cxx::Logger::logger_ = NULL;
#endif

int g_processingCmd = 0;

#ifndef _WIN32

struct flock* file_lock(short type, short whence)
{
    static struct flock ret;
    ret.l_type = type ;
    ret.l_start = 0;
    ret.l_whence = whence;
    ret.l_len = 0;
    ret.l_pid = getpid();
    return &ret;
}
#endif

void saveBackTrace(int signal)
{
#ifndef	WIN32
    time_t tSetTime;
    time( &tSetTime);
    tm* ptm = localtime(&tSetTime) ;
    char fname[256] = {0};
    sprintf(fname, "core.%d_%d_%d_%d_%d_%d",
            ptm->tm_year + 1900, ptm->tm_mon + 1, ptm->tm_mday,
            ptm->tm_hour, ptm->tm_min, ptm->tm_sec);

    FILE* f = fopen(fname, "a");
    if (f == NULL)
    {
        return;
    }
    int fd = fileno(f);
    fcntl(fd, F_SETLKW, file_lock(F_WRLCK, SEEK_SET));

    char buffer[4096] = {0};
    sprintf(buffer, "Dump Time: %d-%d-%d %d:%d:%d\n",
            ptm->tm_year + 1900, ptm->tm_mon + 1, ptm->tm_mday,
            ptm->tm_hour, ptm->tm_min, ptm->tm_sec);
    fwrite(buffer, 1, strlen(buffer), f);

    strcpy(buffer, "Catch signal: ");
    switch (signal)
    {
        case SIGSEGV: strcat(buffer, "SIGSEGV\n");
            break;
        case SIGILL: strcat(buffer, "SIGILL\n");
            break;
        case SIGFPE: strcat(buffer, "SIGFPE\n");
            break;
        case SIGABRT: strcat(buffer, "SIGABRT\n");
            break;
        case SIGTERM: strcat(buffer, "SIGTERM\n");
            break;
        case SIGKILL: strcat(buffer, "SIGKILL\n");
            break;
        case SIGXFSZ: strcat(buffer, "SIGXFSZ\n");
            break;
        default: sprintf(buffer, "Catch signal: %d\n", signal);
            break;
    }
    fwrite(buffer, 1, strlen(buffer), f);

    sprintf(buffer, "Processing cmd: %d\n", g_processingCmd);
    fwrite(buffer, 1, strlen(buffer), f);

    struct sysinfo s_info;
    int error;
    error = sysinfo(&s_info);
    sprintf(buffer, "code error=[%d]\nUptime =[%ld]s\nLoad: 1 min[%lu] / 5 min[%lu] / 15 min[%lu]"\
		"\nRAM: total[%lu] / free[%lu] / shared[%lu]\n Memory in buffers =[%lu]\nSwap:total[%lu]/free[%lu]"\
		"\nNumber of processes =[%d]\n\n",
            error, s_info.uptime, s_info.loads[0], s_info.loads[1], s_info.loads[2],
            s_info.totalram, s_info.freeram, s_info.sharedram, s_info.bufferram,
            s_info.totalswap, s_info.freeswap, s_info.procs);

    void* DumpArray[256];
    int	nSize =	backtrace(DumpArray, 256);
    char** symbols = backtrace_symbols(DumpArray, nSize);
    if (symbols)
    {
        if (nSize > 256)
        {
            nSize = 256;
        }
        if (nSize > 0)
        {
            for (int	i = 0; i < nSize; i++)
            {
                fwrite(symbols[i], 1, strlen(symbols[i]), f);
                fwrite("\n", 1, 1, f);
            }
        }
        free(symbols);
    }
    fcntl(fd, F_SETLK, file_lock(F_UNLCK, SEEK_SET));
    fclose(f);

    exit(1);
#endif
}

ServerConfig serverConfig("config/server.cfg");

Daemon::Daemon(int nid)
{
    char logger[1024] = {0};
    sprintf(logger, "logger_game%d.cfg", nid);
    log4cxx::PropertyConfigurator::configureAndWatch(logger);
	GameConfigMgr::CreateInst()->LoadRuntimeConfig();

    if (serverConfig.useShareMem())
    {
        //启动共享内存
        int nKey = 21641000 + nid;
#ifndef WIN32
        //windows下启动共享内存，必须另外启动一个进程，使用相同的共享内存，保证数据不丢失！
        if (!ShareMemHandler::GetInst()->Init("gamed", nid, 1024 * 1024 * 1024, nKey))
        {
            LOG4CXX_ERROR(logger_, "Root ShareMem Error!!!");
        }
#endif
    }

    eq = new EventQueue();
    TEST[E_EVENT_QUEUE]++;
    nh = new GameNetHandler(eq, nid);
    TEST[E_GAME_NET_HANDLER]++;
    dh = new GameDataHandler(nid);
    TEST[E_GAME_DATE_HANDLER]++;
    eh = new GameEventHandler(eq, dh, nh, nid);
    TEST[E_GAME_EVENT_HANDLER]++;
    logger_ = log4cxx::Logger::getLogger("Daemon");

    UserCtrl::SetEventHandler(eh);
   	GameConfigMgr::CreateInst()->LoadGameConfigInfo();
	
    ///*Rect * pRect = */pMap->FindPath();
    //test code over..
    //是否启动
    if (serverConfig.IsGameLogStart(nid))
    {
        CSysLog::GetInstance()->SetLogInfo(true, nid, serverConfig.GetGameLogDir(nid), serverConfig.GetGameLogName(nid),
                serverConfig.GetGameLog2SrvAddr(nid), serverConfig.GetGameLog2SrvPort(nid), serverConfig.IsGameLogStart(nid),
                serverConfig.GetGameLogStatLv(nid), serverConfig.GetGameLogStatModul(nid), serverConfig.GetGameLogStatModulVal(nid));
    }
}

Daemon::~Daemon()
{
    delete nh;
    delete dh;
    delete eh;
    delete eq;
    TEST[E_GAME_NET_HANDLER]--;
    TEST[E_GAME_DATE_HANDLER]--;
    TEST[E_GAME_EVENT_HANDLER]--;
    TEST[E_EVENT_QUEUE]--;
}

void Daemon::start()
{
    eh->start();
    nh->start();
}

int main(int argn, char *argv[]) // here is the entrance of daemon
{

#ifdef _WIN32
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);
#endif

    // dump process id
#ifndef _WIN32
    pid_t pid = getpid();
    char filebuff[256];
    sprintf(filebuff, "%s%s.pid", argv[0], argv[1]);
    FILE * fp = fopen(filebuff, "w");
    fprintf(fp, "%d\n", pid);
    fclose(fp);

    signal(SIGSEGV, saveBackTrace);
    signal(SIGILL, saveBackTrace);
    signal(SIGFPE, saveBackTrace);
    signal(SIGABRT, saveBackTrace);
    signal(SIGTERM, saveBackTrace);
    signal(SIGKILL, saveBackTrace);
    signal(SIGXFSZ, saveBackTrace);

    // block SIGINT to all child process:
    sigset_t bset, oset;
    sigemptyset(&bset);
    sigaddset(&bset, SIGINT);
    // equivalent to sigprocmask
    if (pthread_sigmask(SIG_BLOCK, &bset, &oset) != 0)
    {
        printf("set thread signal mask error!");
        return 0;
    }
#endif
    GDHandler::GetInstance()->Init("./verdanab.ttf", "*", 100, 40);
    if (argn != 2)
    {
        printf("usage: gamed node_id\n");
        getchar();
    }
    int nid;
    safe_atoi(argv[1], nid);
    Daemon daemon(nid);
    printf("Daemon init finished!\n");

    daemon.start();
    printf("Daemon started!\n");

    GDHandler::GetInstance()->Release();
    return 0;
}
