# [Redis Windows Service](https://github.com/chaosannals/rws)

## 使用

在 [GitHub Releases](https://github.com/chaosannals/rws/releases) 找到最新版本的 msi 文件，下载并安装。

安装完成之后就可以看到服务里多了个 Redis Windows Service ，可以关闭和启动 Redis 功能。

## 编译过程

Redis 官网下载 Redis 源码 redis-6.2.2.tar.gz
MSYS2 官网下载并安装 MSYS2

### 一些修改

MSYS2 目录下 /usr/include/dlfcn.h 修改源码
注释掉 宏判定 #if __GNU_VISIBLE #endif
不然会报找不到结构 Dl_info 错误。

注：编译完记得改回去。

### 开始编译

```bash
# 更新 MSYS2 系统
pacman -Syu

# 安装 gcc make pkg-config
pacman -Sy gcc make pkg-config

# 到编译的目录（MSYS2 /d 就是 D盘；/e 就是 E盘）
cd /d/redis

# 解压 redis 源码
tar -xvf redis-6.2.2.tar.gz

# 进入原码目录
cd redis-6.2.2

# 编译
make PREFIX=/d/redis/dist install
```


## 奇怪的 bug

注：在后面的版本里，编译的 redis 路径必须带有 dist 不然就会报错。不知道是不是 MSYS2 的什么问题。