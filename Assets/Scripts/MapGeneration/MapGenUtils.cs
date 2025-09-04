using System.Linq;
using System.Collections.Generic;

public enum RotationalAxis {N,X,Y,Z}
public static class MapGenUtils{

    public static int MapGenRandom(this double[] a, double r){
        double sum = a.Sum();

		if (sum == 0)
		{
			for (int j = 0; j < a.Count(); j++) a[j] = 1;
			sum = a.Sum();
		}

		for (int j = 0; j < a.Count(); j++) a[j] /= sum;

		int i = 0;
		double x = 0;

		while (i < a.Count())
		{
			x += a[i];
			if (r <= x) return i;
			i++;
		}

		return 0;
    }

    public static Sockets RotateSockets(Sockets sock2rot, int rot, RotationalAxis axis){
        if(axis == RotationalAxis.X){
            //Default Sockets   ==  Sockets(sock2rot.posX, sock2rot.negX, sock2rot.posY, sock2rot.negY, sock2rot.posZ, sock2rot.negZ);}
            if(rot == 90){return new Sockets(sock2rot.posX, sock2rot.negX, sock2rot.negZ, sock2rot.posZ, sock2rot.posY, sock2rot.negY);}
            if(rot == 180){return new Sockets(sock2rot.posX, sock2rot.negX, sock2rot.negY, sock2rot.posY, sock2rot.negZ, sock2rot.posZ);}
            if(rot == 270){return new Sockets(sock2rot.posX, sock2rot.negX, sock2rot.posZ, sock2rot.negZ, sock2rot.negY, sock2rot.posY);}
        }
        if(axis == RotationalAxis.Y){
            //Default Sockets   ==  Sockets(sock2rot.posX, sock2rot.negX, sock2rot.posY, sock2rot.negY, sock2rot.posZ, sock2rot.negZ);}
            if(rot == 90){return new Sockets(sock2rot.posZ, sock2rot.negZ, sock2rot.posY, sock2rot.negY, sock2rot.negX, sock2rot.posX);}
            if(rot == 180){return new Sockets(sock2rot.negX, sock2rot.posX, sock2rot.posY, sock2rot.negY, sock2rot.negZ, sock2rot.posZ);}
            if(rot == 270){return new Sockets(sock2rot.negZ, sock2rot.posZ, sock2rot.posY, sock2rot.negY, sock2rot.posX, sock2rot.negX);}
        }
        if(axis == RotationalAxis.Z){
            //Default Sockets   ==  Sockets(sock2rot.posX, sock2rot.negX, sock2rot.posY, sock2rot.negY, sock2rot.negZ, sock2rot.posZ);}
            if(rot == 90){return new Sockets(sock2rot.posY, sock2rot.negY, sock2rot.negX, sock2rot.posX, sock2rot.posZ, sock2rot.negZ);}
            if(rot == 180){return new Sockets(sock2rot.negX, sock2rot.posX, sock2rot.negY, sock2rot.posY, sock2rot.posZ, sock2rot.negZ);}
            if(rot == 270){return new Sockets(sock2rot.negY, sock2rot.posY, sock2rot.posX, sock2rot.negX, sock2rot.posZ, sock2rot.negZ);}
        }
        return sock2rot;
    }
    public static Sockets SelfSocket(string NFID) => new Sockets(new face(NFID),new face(NFID),new face(NFID),new face(NFID),new face(NFID),new face(NFID));
        /*face[] emptyFace = new face[6];
        for(int f = 0; f < 6; f++){
            emptyFace[f] = new face(NFID);
            emptyFace[f].sym = false;
        }*/
}

[System.Serializable]
public class ListThatIsAClass<T>{ //for when you need a list that acts like a class
    public List<T> list = new List<T>();
    public T this[int key]{
        get => list[key];
        set => list[key] = value;
    }
}

[System.Serializable]
public class KVPair<K,V>{
    public K Key;
    public V Val;
    public KVPair(K k, V v){
        Key = k;
        Val = v;
    }
}