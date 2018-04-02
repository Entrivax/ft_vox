.PHONY: all clean fclean re start

all:
	@xbuild "/p:Configuration=Release" "/target:build"

start: all
	@cd ft_vox/bin/Release && mono ft_vox.exe

clean:
	@xbuild "/p:Configuration=Release" "/target:clean"

fclean: clean

re:
	@xbuild "/p:Configuration=Release" "/target:clean;build"
